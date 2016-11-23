//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Users.Dal
{
    using Lomo.AzureCaching;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure;
    //using System.Data.Objects;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text;
    using Lomo.Logging;
    using Microsoft.Practices.TransientFaultHandling;
    using Newtonsoft.Json;
    using Users.Dal.DataModel;
    using System.Data.Entity.Core.Objects;

    /// <summary>
    ///     The users dal.
    ///     Only one instance should be used for application
    /// </summary>
    public class UsersDal : IUsersDal
    {
        #region Fields

        /// <summary>
        ///     The partitioner.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        private readonly Partitioner partitioner = new Partitioner();

        /// <summary>
        /// The confirmation jobs queue.
        /// </summary>
        private readonly IPriorityEmailJobsQueue<PriorityEmailCargo> confirmationJobsQueue;

        /// <summary>
        ///     The connection string to the users database. If null the default one will be used
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The retry policy.
        /// </summary>
        private readonly RetryPolicy retryPolicy;

        /// <summary>
        /// Should cache objects retrieved from DB
        /// </summary>
        private readonly bool cacheObjects;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersDal"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="retryPolicy">
        /// The retry Policy.
        /// </param>
        /// <param name="cacheObjects">
        /// Should cache objects retrieved from DB
        /// </param>
        /// <param name="queue">
        /// The queue.
        /// </param>
        public UsersDal(string connectionString = null, RetryPolicy retryPolicy = null, bool cacheObjects = false, IPriorityEmailJobsQueue<PriorityEmailCargo> queue = null)
        {
            this.connectionString = connectionString;
            this.confirmationJobsQueue = queue ?? new PriorityEmailJobsQueue<PriorityEmailCargo>();
            this.retryPolicy = retryPolicy ?? RetryPolicyProvider.GetRetryPolicy();
            this.cacheObjects = cacheObjects;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// if the user with the given address doesn't exist, it will be created.
        ///     The user object for the newly created user or the already exists user will be returned.
        /// </summary>
        /// <param name="email">
        /// The email address.
        /// </param>
        /// <param name="isEmailConfirmed">
        /// if true - this method is being called in the context where the email address was confirmed. 
        /// </param>
        /// <param name="userSource">
        /// Source of the email address
        /// </param>
        /// <param name="userLocation">User location info from frontdoor serialized as a Base64 encoded string</param>
        /// <returns>
        /// The <see cref="User"/>. object
        /// </returns>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        public User CreateOrGetUserByEmail(string email, bool isEmailConfirmed, string userSource = null, string userLocation = null)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("email can't be null or empty", "email");
            }

            email = email.ToLowerInvariant().Trim();
            
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(userSource))
            {
                sb.Append(userSource);
            }

            if (!string.IsNullOrEmpty(userLocation))
            {
                try
                {
                    UserLocation fdLocation = UserLocation.DeSerializeModel(userLocation);
                    string userInfoJson = JsonConvert.SerializeObject(fdLocation);
                    if (string.IsNullOrEmpty(userSource) || !userSource.Contains("#"))
                    {
                        sb.Append("##");
                    }
                    else
                    {
                        sb.Append("#");
                    }
                    sb.Append(userInfoJson);
                }
                catch(Exception exception)
                {
                    //We will just ignore the error if something goes wrong serializing/deserializing the user location. Location is not mandatory for creating the user.

                    Log.Warn("Unable to get FDLocation for user [email:{0}, userSource:{1}, userLocation:{2}, Error:{3}]", email, userSource, userLocation, exception.Message);
                }
            }
            if (sb.Length > 0)
            {
                userSource = sb.ToString();
            }
            Guid userId = this.Execute(
                email,
                (context, partitionId) =>
                {
                    ObjectResult<ExternalUserDb> extUsers = context.CreateNotExistingExternalUser(
                        null, email, partitionId, (int)UserExternalIdType.Email);
                    ExternalUserDb extUser = extUsers.First();
                    return extUser.UserId;
                });

            // If the user doesn't exists it will be created with the related email address. Otherwise the existing user will be returned
            UserDb res = this.Execute(
                userId.ToString(),
                (context, partitionId) =>
                context.CreateOrUpdateUser(id: userId, partitionId: partitionId, msId: null, email: email, phoneNumber: null, name: null, json: null, source: userSource, isEmailConfirmed: isEmailConfirmed).First());
            
            return DataContractConverters.Convert(res);
        }

        /// <summary>
        /// if the user with the given microsoft id doesn't exist, it will be created.
        ///     The user object for the newly created user or the already existing user will be returned.
        /// </summary>
        /// <param name="microsoftId">
        /// The user's microsoft id.
        /// </param>
        /// <param name="userSource">
        /// Source of the email address
        /// </param>
        /// <param name="userName">
        /// The user's name.
        /// </param>
        /// <param name="userId">the userId (optional)</param>
        /// <param name="userLocation">User location info from frontdoor serialized as a Base64 encoded string</param>
        /// <returns>
        /// The <see cref="User"/>. object
        /// </returns>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <param name="microsoftId">
        /// </param>
        /// is null or emtpy string
        /// </exception>
        public User CreateOrGetUserByMsId(string microsoftId, string userSource = null, string userName = null, Guid? userId = null, UserLocation userLocation = null)
        {
            if (string.IsNullOrEmpty(microsoftId))
            {
                throw new ArgumentException("microsoftId can't be null or empty", "microsoftId");
            }

            microsoftId = microsoftId.ToLowerInvariant();
            var user = cacheObjects ? CacheItem.Get(microsoftId, () => CreateOrGetUserByMsIdFromDb(microsoftId, userSource, userName, userId, userLocation)) : CreateOrGetUserByMsIdFromDb(microsoftId, userSource, userName, userId, userLocation);
            return user;
        }

        /// <summary>
        /// The update email subscriptions. Will overwrite the existing list of locations of the user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="locationIds">
        /// The location ids.
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <exception cref="ArgumentNullException"> locationIds are null </exception>
        public void UpdateEmailSubscriptions(Guid userId, IEnumerable<string> locationIds, SubscriptionType subscriptionType)
        {
            if (locationIds == null)
            {
                throw new ArgumentNullException("locationIds");
            }

            List<string> uniqueLocations = new HashSet<string>(locationIds).ToList();

            this.Execute(
                userId.ToString(),
                (context, partitionId) =>
                {
                    DbCommand command = context.Database.Connection.CreateCommand();
                    command.CommandText = "UpdateEmailSubscriptions";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.UniqueIdentifier, Value = userId });
                    command.Parameters.Add(new SqlParameter { ParameterName = "@PartitionId", SqlDbType = SqlDbType.Int, Value = partitionId });
                    this.AddSubscriptionListParameter(command, "@SubscriptionsList", uniqueLocations);
                    command.Parameters.Add(new SqlParameter { ParameterName = "@SubscriptionType", SqlDbType = SqlDbType.NVarChar, Value = subscriptionType });
                    command.ExecuteNonQuery();
                    return 0;
                });
        }

        /// <summary>
        /// Create or update email subscription
        /// </summary>
        /// <param name="emailSubscription">
        /// The create Or Update.
        /// </param>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        public void CreateOrUpdateEmailSubscription(EmailSubscription emailSubscription)
        {
            this.Execute(
                emailSubscription.UserId.ToString(),
                (context, partitionId) =>
                        context.CreateOrUpdateEmailSubscription(emailSubscription.UserId, partitionId, emailSubscription.LocationId, emailSubscription.IsActive, emailSubscription.SubscriptionType.ToString()));
        }

        /// <summary>
        /// The create or update unsubscribe url.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="unsubscribeUrl">
        /// The unsubscribe url.
        /// </param>
        public void CreateOrUpdateUnsubscribeUrl(Guid userId, string unsubscribeUrl)
        {
            this.Execute(
                userId.ToString(),
                (context, partitionId) =>
                {
                    context.CreateOrUpdateEmailUnsubscribeUrl(userId, partitionId, unsubscribeUrl);
                    return 0;
                });
        }

        /// <summary>
        /// Deletes email subscription
        /// </summary>
        /// <param name="userId">
        /// internal user id
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <param name="locationId">
        /// the location id. if null, all the email subscriptions of this user will be deleted
        /// </param>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        public void DeleteEmailSubscriptions(Guid userId, SubscriptionType subscriptionType, string locationId = null)
        {
            this.Execute(
                userId.ToString(),
                (context, partitionId) => context.DeleteEmailSubscriptions(userId, partitionId, locationId, subscriptionType.ToString()));
        }

        /// <summary>
        /// Gets email subscriptions by user id
        /// </summary>
        /// <param name="userId">
        /// internal user id
        /// </param>
        /// <param name="isActive">
        /// isActive = true: returns only active subscriptions
        ///     isActive = false: returns only disabled subscriptions
        ///     isActive = null: returns all the subscriptions
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <returns>
        /// list of email subscriptions
        /// </returns>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        public IEnumerable<EmailSubscription> GetEmailSubscriptionsByUserId(Guid userId, bool? isActive, string subscriptionType = null)
        {
            return this.Execute(
                userId.ToString(),
                (context, partitionId) =>
                context.GetEmailSubscriptionsByUserId(userId, partitionId, isActive, subscriptionType)
                       .Select(DataContractConverters.Convert)
                       .ToList());
        }

        /// <summary>
        /// Gets Email subscriptions for a given email address
        /// </summary>
        /// <param name="emailAddress">Email address to look up</param>
        /// <param name="isActive"> 
        /// isActive = true: returns only active subscriptions 
        /// isActive = false: returns only disabled subscriptions
        /// isActive = null: returns all the subscriptions</param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user </param>
        /// <returns>list of email subscriptions</returns>
        public IEnumerable<EmailSubscription> GetEmailSubscriptionsByEmail(string emailAddress, bool? isActive,
            string subscriptionType = null)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentException("Email address cannot be empty", "emailAddress");
            }

            emailAddress = emailAddress.ToLowerInvariant().Trim();
            return this.Execute(
                context =>
                context.GetEmailSubscriptionsByEmail(emailAddress, isActive, subscriptionType)
                       .Select(DataContractConverters.Convert)
                       .ToList());
        }

        /// <summary>
        /// Gets subscriptions in batches
        /// </summary>
        /// <param name="takeCount">
        /// the maximum number of elements that will be returned in this batch request
        /// </param>
        /// <param name="isActive">
        /// isActive = true: returns only active subscriptions
        ///     isActive = false: returns only disabled subscriptions
        ///     isActive = null: returns all the subscriptions
        /// </param>
        /// <param name="continuationContext">
        /// If null - will start from the beginning of the subscriptions list
        ///     Otherwise - the continuationContext should be the one that was returned in the previous call to this method
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <returns>
        /// subscriptions batch response. The response includes:
        ///     - List of subscriptions
        ///     - Continuation Context object that should be passed in the next call for this method
        ///     - Parameter that indicates if there are more subscriptions
        /// </returns>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <param name="continuationContext">
        /// </param>
        /// is illegel
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The operation is invald. This can happen if a lot of time passed between the subsequent batch requests or the end of list reached in a previous call
        /// </exception>
        public EmailsSubscriptionsBatchResponse GetNextEmailSubscriptionsBatch(
            int takeCount, bool? isActive, object continuationContext, SubscriptionType subscriptionType)
        {
            if (continuationContext == null)
            {
                var context = new EmailSubscriptionsBatchContext
                                  {
                                      FromLocationId = null,
                                      FromPartitionId = null,
                                      FromUserId = null,
                                      PartitionIndex = 0,
                                      HasMore = true
                                  };
                return this.GetNextEmailSubscriptionsBatch(takeCount, isActive, context, subscriptionType);
            }

            if (!(continuationContext is EmailSubscriptionsBatchContext))
            {
                throw new ArgumentException(
                    "Continuation Context must be null or the previous one", "continuationContext");
            }

            return this.GetNextEmailSubscriptionsBatch(
                takeCount, isActive, (EmailSubscriptionsBatchContext)continuationContext, subscriptionType);
        }

        /// <summary>
        /// Gets email subscriptions by location id
        /// </summary>
        /// <param name="locationId">
        /// the location id
        /// </param>
        /// <param name="isActive">
        /// isActive = true: returns only active subscriptions
        ///     isActive = false: returns only disabled subscriptions
        ///     isActive = null: returns all the subscriptions
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <returns>
        /// list of email subscriptions
        /// </returns>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        public IEnumerable<EmailSubscription> GetSubscriptionsByLocationId(string locationId, bool? isActive, SubscriptionType subscriptionType)
        {
            List<EmailSubscription> result = null;

            Parallel.For(
                0,
                1,
                index => this.Execute(
                    context =>
                    result =
                    context.GetEmailSubscriptionsByLocationId(locationId, isActive, subscriptionType.ToString())
                           .Select(DataContractConverters.Convert)
                           .ToList()));

            return result;
        }

        /// <summary>
        /// The get unsubscribe info to update.
        /// </summary>
        /// <param name="updatedBefore">
        /// The updated before.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<EmailUnsubscribeInfo> GetUnsubscribeInfoToUpdate(DateTime updatedBefore)
        {
            List<EmailUnsubscribeInfo> result = null;

            Parallel.For(
                0,
                1,
                index => this.Execute(
                    context =>
                    result =
                    context.GetEmailUnsubscribeUrlToUpdate(updatedBefore)
                           .Select(DataContractConverters.Convert)
                           .ToList()));

            return result;
        }

        /// <summary>
        /// The get unsubscribe url info.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="EmailUnsubscribeInfo"/>.
        /// </returns>
        public EmailUnsubscribeInfo GetUnsubscribeUrlInfo(Guid userId)
        {
            return this.Execute(
                userId.ToString(),
                (context, partitionId) =>
                {
                    EmailUnsubscribeUrl_Result dbObject =
                        context.GetEmailUnsubscribeUrlByUserId(userId, partitionId).FirstOrDefault();
                    return DataContractConverters.Convert(dbObject);
                });
        }

        /// <summary>
        /// gets user by external id
        /// </summary>
        /// <param name="externalId">
        /// The external id
        /// </param>
        /// <param name="externalIdType">
        /// The external Id Type.
        /// </param>
        /// <returns>
        /// the user object if exists, otherwise returns null
        /// </returns>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        public User GetUserByExternalId(string externalId, UserExternalIdType externalIdType)
        {
            if (string.IsNullOrEmpty(externalId))
            {
                throw new ArgumentNullException("externalId");
            }

            externalId = externalId.ToLowerInvariant();
            Guid? userId = this.Execute(
                externalId,
                (context, partitionId) =>
                context.GetUserIdByExternalId(externalId, partitionId, (int)externalIdType).FirstOrDefault());
            return userId != null ? this.GetUserByUserId(userId.Value) : null;
        }

        /// <summary>
        /// gets user by internal id
        /// </summary>
        /// <param name="id">
        /// internal user id
        /// </param>
        /// <returns>
        /// the user object if exists, otherwise returns null
        /// </returns>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        public User GetUserByUserId(Guid id)
        {
            return this.Execute(
                id.ToString(),
                (context, partitionId) =>
                {
                    UserDb dbUser = context.GetUserByUserId(id, partitionId).FirstOrDefault();
                    return DataContractConverters.Convert(dbUser);
                });
        }

        /// <summary> The create confirmation code - simplified  </summary>
        /// <param name="user"> The user object.
        /// </param>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="maxRetryCount"> The max retry count. if not specified the default value will be used
        /// </param>
        /// <param name="codeExpirationWindow"> The code expiration window. If not specified the default value will be used 
        /// </param>
        /// <returns> A tuple of user id hash and a confirmation code  </returns>
        /// <exception cref="ArgumentException"> Invalid input argument  </exception>
        /// <exception cref="EntityException"> Any Entities framework related error </exception>
        /// <exception cref="SqlException"> Any sql related error </exception>
        /// <exception cref="NotSupportedException"> entity type isn't EmailAddress or AccountLink or environment isn't Integration or Production </exception>
        public Tuple<string, int> CreateConfirmationCode(User user, EntityType entityType, int? maxRetryCount = null, TimeSpan? codeExpirationWindow = null)
        {
            switch (entityType)
            {
                case EntityType.AuthenticatedEmailAddress:
                    return this.CreateConfirmationCode(user.Email, EntityType.AuthenticatedEmailAddress, user.Id, maxRetryCount, codeExpirationWindow);
                case EntityType.UnAuthenticatedEmailAddress:
                    return this.CreateConfirmationCode(user.Email, EntityType.UnAuthenticatedEmailAddress, user.Id, maxRetryCount, codeExpirationWindow);

                case EntityType.AccountLink:
                    return this.CreateConfirmationCode(user.Id.ToString(), EntityType.AccountLink, user.Id, maxRetryCount, codeExpirationWindow);
                default:
                    throw new NotSupportedException(string.Format("The only supported entities are AuthenticatedEmailAddress, UnAuthenticatedEmailAddress and AccountLink. {0} isn't supported", entityType));
            }
        }

        /// <summary> returns a tuple of user id hash and a confirmation code  </summary>
        /// <param name="entity"> The entity.  </param>
        /// <param name="entityType"> The entity type.  </param>
        /// <param name="userId"> The user id.  </param>
        /// <param name="maxRetryCount"> The max retry count.  </param>
        /// <param name="codeExpirationWindow"> The code Expiration Window. </param>
        /// <returns> The <see cref="int"/>.  </returns>
        /// <exception cref="ArgumentException"> Invalid input argument   </exception>
        /// <exception cref="EntityException"> Any Entities framework related error  </exception>
        /// <exception cref="SqlException"> Any sql related error  </exception>
        public Tuple<string, int> CreateConfirmationCode(string entity, EntityType entityType, Guid userId, int? maxRetryCount = null, TimeSpan? codeExpirationWindow = null)
        {
            maxRetryCount = maxRetryCount.HasValue ? maxRetryCount.Value : 20;
            codeExpirationWindow = codeExpirationWindow.HasValue ? codeExpirationWindow.Value : TimeSpan.FromDays(3);
            DateTime expirationDate = DateTime.UtcNow.Add(codeExpirationWindow.Value);

            if (string.IsNullOrEmpty(entity))
            {
                throw new ArgumentException("entity can't be empty or null", "entity");
            }

            if (entityType == EntityType.None)
            {
                throw new ArgumentException(string.Format("Invalid Entity Type:{0}", entityType), "entityType");
            }

            if (maxRetryCount < 1)
            {
                throw new ArgumentException(string.Format("Invalid Max Retry Count:{0}", entityType), "maxRetryCount");
            }

            string userIdHash = SecureHashGenerator.Generate(userId.ToString());
            entity = entity.Trim().ToLowerInvariant();
            var result = this.Execute(
            userIdHash,
            (context, partitionId) => context.CreateConfirmationCode(userIdHash, partitionId, entity, (byte)entityType, userId, maxRetryCount.Value, expirationDate).First());

            return new Tuple<string, int>(userIdHash, result.Code);
        }

        /// <summary> The log user confirm email resend. </summary>
        /// <param name="userId"> The user id. </param>
        /// <param name="entityType"> The entity type. </param>
        public void LogUserConfirmEmailResend(Guid userId, EntityType entityType)
        {
            this.Execute(
                userId.ToString(),
                (context, partitionId) => context.LogUserConfirmEmailResend(userId, partitionId, (byte)entityType));
        }

        /// <summary> The get user confirm email resend count. </summary>
        /// <param name="userId"> The user id. </param>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="fromDateTime"> The from date time. </param>
        /// <returns> The <see cref="int"/>. </returns>
        public int GetUserConfirmEmailResendCount(Guid userId, EntityType entityType, DateTime fromDateTime)
        {
            var result = this.Execute(
                userId.ToString(),
                (context, partitionId) => context.GetConfirmEmailResendCountByUser(userId, partitionId, (byte)entityType, fromDateTime).First());
            return result == null ? 0 : result.Value;
        }

        /// <summary>
        /// Confirm entity.
        /// </summary>
        /// <param name="userIdHash">
        /// The user Id Hash.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="confirmationCode">
        /// The confirmation code.
        /// </param>
        /// <returns>
        /// The <see cref="ConfirmEntityResult"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Invalid input argument 
        /// </exception>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        /// <exception cref="SqlException">
        /// Any sql related error
        /// </exception>
        public ConfirmEntityResult ConfirmEntity(string userIdHash, EntityType entityType, int confirmationCode)
        {
            if (string.IsNullOrEmpty(userIdHash))
            {
                throw new ArgumentException("User Id hash can't be empty or null", "userIdHash");
            }

            if (entityType == EntityType.None)
            {
                throw new ArgumentException(string.Format("Invalid Entity Type:{0}", entityType), "entityType");
            }

            var databaseResult = this.Execute(
                userIdHash,
                (context, partitionId) => context.ConfirmEntity(userIdHash, partitionId, (byte)entityType, confirmationCode).First());
            ConfirmEntityResult result = new ConfirmEntityResult();
            if (string.IsNullOrEmpty(databaseResult.EntityId) && databaseResult.UserId == null)
            {
                result.Status = ConfirmStatus.CodeNotFound;
            }
            else
            {
                if (databaseResult.IsValid == 0)
                {
                    result.Status = ConfirmStatus.Invalid;
                }
                else if (databaseResult.IsConfirmed == 0)
                {
                    result.Status = ConfirmStatus.CodeWrong;
                }
                else
                {
                    result.Status = ConfirmStatus.CodeConfirmed;
                    result.UserId = databaseResult.UserId;
                    result.EntityId = databaseResult.EntityId;
                }
            }

            return result;
        }

        /// <summary>
        /// The get confirm entity.
        /// </summary>
        /// <param name="userIdHash"> The user id hash. </param>
        /// <param name="entityType"> The entity type. </param>
        /// <returns> The <see cref="string"/>. entity id
        /// </returns>
        /// <exception cref="ArgumentException"> 
        /// userIdHash is null or empty.
        /// EntityType is None
        /// </exception>
        public ConfirmEntity GetConfirmationEntity(string userIdHash, EntityType entityType)
        {
            if (string.IsNullOrEmpty(userIdHash))
            {
                throw new ArgumentException("User Id hash can't be empty or null", "userIdHash");
            }

            if (entityType == EntityType.None)
            {
                throw new ArgumentException(string.Format("Invalid Entity Type:{0}", entityType), "entityType");
            }

            var databaseResult = this.Execute(
                userIdHash,
                (context, partitionId) => context.GetConfirmEntity(userIdHash, partitionId, (byte)entityType).SingleOrDefault());
            if (databaseResult == null)
            {
                return null;
            }

            return new ConfirmEntity { UserId = databaseResult.UserId, CreatedDate = databaseResult.CreatedDate, Name = databaseResult.EntityId, Type = entityType };
        }

        /// <summary>
        /// update user suppress information </summary>
        /// <param name="email"> The user's email. </param>
        /// <param name="isSuppressed">The user is suppressed or un-suppressed</param>
        /// <returns> The suppressed user object </returns>
        /// <exception cref="EntityException"> Any Entities framework related error </exception>
        /// <exception cref="ArgumentException"> <param name="email"> </param> is null or emtpy string </exception>
        public User UpdateUserSuppressInfo(string email, bool isSuppressed)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("email must not be empty or null");
            }

            email = email.ToLowerInvariant().Trim();

            Guid? userId;
            if (isSuppressed)
            {
                // Get the user internal id. If not exists - Force creation
                userId = this.Execute(
                    email,
                    (context, partitaionId) =>
                    {
                        var extUsers = context.CreateNotExistingExternalUser(null, email, partitaionId, (int)UserExternalIdType.Email);
                        var extUser = extUsers.First();
                        return extUser.UserId;
                    });
            }
            else
            {
                // Get the user internal id. If not exists we won't force creation
                userId = this.Execute(
                email,
                (context, partitionId) => context.GetUserIdByExternalId(email, partitionId, (int)UserExternalIdType.Email).FirstOrDefault());
            }

            // Updates the is suppressed flag only if this is suppressed operation or if the user/ email already exists in the system
            if (isSuppressed || userId.HasValue)
            {
                var res = this.Execute(userId.ToString(), (context, partationId) => context.UpdateUserSuppressInfo(userId, partationId, email, isSuppressed).First());
                return DataContractConverters.Convert(res);
            }

            // If the user doesn't exists and this is an unsuppressed operation this is actually non-op operation. The user wasn't created, null will be returned
            return null;
        }

        /// <summary>
        /// Update the user's email address
        /// </summary>
        /// <remarks>
        /// -If the user (the target user) is a suppressed user or doesn't have an associated microsoft id the operation isn't allowed and an exception will be thrown.
        ///     -If a user with the same email address (the source user) already exists an attempt to merge the users will be done:
        ///     -If the source user has an associated microsoft id or is suppressed user the operation isn't allowed and an exception will be thrown
        ///     -otherwise the subscriptions of the source user will be attached to the subscriptions of the target user and the source will be deleted from the system (only if the target user doesn't have subscriptions)
        /// </remarks>
        /// <param name="userId">
        /// The id of the target user.
        /// </param>
        /// <param name="email">
        /// The new email address.
        /// </param>
        /// <param name="isEmailConfirmed">if true - this method is being called in the context where the email address was confirmed. </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// email is null or empty
        /// </exception>
        /// <exception cref="UserNotExistsException">
        /// the target user doesn't exist in the system
        /// </exception>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        public User UpdateUserEmail(Guid userId, string email, bool isEmailConfirmed)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email can't be null or empty", "email");
            }

            email = email.ToLowerInvariant().Trim();

            User user = this.GetUserByUserId(userId);
            if (user == null)
            {
                throw new UserNotExistsException(string.Format("User with id: {0} doesn't exist in the system", userId));
            }

            if (user.Email == email)
            {
                if (user.IsEmailConfirmed || user.IsEmailConfirmed == isEmailConfirmed)
                {
                    // Do nothing the user already have this email address + the email is already confirmed or confirmed flag equals to the input (false)
                    return user;
                }

                // Update isEmailConfirmed
                UserDb res = this.Execute(
                    userId.ToString(),
                    (context, partitionId) => context.CreateOrUpdateUser(
                        id: userId, partitionId: partitionId, msId: null, email: null, phoneNumber: null, name: null, json: null, source: null, isEmailConfirmed: isEmailConfirmed).First());
                return DataContractConverters.Convert(res);
            }

            if (user.IsSuppressed)
            {
                throw new UserUpdateConflictException(
                    string.Format("Can't update email address for suppressed user. User Id: {0}", user.Id));
            }

            if (user.MsId == null)
            {
                throw new UserUpdateConflictException(
                    string.Format("Can't update email address for user without microsoft id. User Id: {0}", user.Id));
            }

            // Check if we have a user with this email
            User emailUser = this.GetUserByExternalId(email, UserExternalIdType.Email);
            if (emailUser != null)
            {
                if (emailUser.IsSuppressed)
                {
                    // This is a suppressed email. Don't allow this operation
                    throw new UserUpdateConflictException(
                        string.Format(
                            "Can't update email address for user because the email is already associated with suppressed user. User Id: {0}, Email: {1}",
                            user.Id,
                            email));
                }

                if (emailUser.Id == user.Id && emailUser.Email == email)
                {
                    // The external users table and the users table are already updated with the new email information for the user (probably race condition from other process / thread)
                    // Just return the updated user.
                    return emailUser;
                }

                if (emailUser.Id == user.Id)
                {
                    // The external users table is already updated with this information (probably race condition from other process / thread)
                    // Make sure that the entire 'UpdateEmail' operation is completed
                    return this.ChangeUserDbEmail(user, email, isEmailConfirmed);
                }

                if (emailUser.MsId != null)
                {
                    throw new UserUpdateConflictException(
                        string.Format(
                            "Can't update email address for user because the email is already associated with other microsoft id. User Id: {0}, Email: {1}",
                            user.Id,
                            email));
                }

                // If we arrived here we can continue with the merge operation
                this.CopyUserSubscriptions(emailUser, user);
                this.DeleteEmailUser(emailUser);
            }

            return this.ChangeUserDbEmail(user, email, isEmailConfirmed);
        }

        /// <summary>
        /// Update the user's phone number
        /// </summary>
        /// <param name="userId"> The id of the target user. </param>
        /// <param name="phoneNumber"> The new phone number. </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        /// <exception cref="ArgumentException"> phone number is null or empty </exception>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        /// <exception cref="SqlException">Any sql  related error</exception>
        public User UpdateUserPhoneNumber(Guid userId, string phoneNumber)
        {
            if (phoneNumber == null)
            {
                throw new ArgumentNullException("phoneNumber");
            }

            UserDb res = this.Execute(
                userId.ToString(),
                (context, partitionId) =>
                context.CreateOrUpdateUser(id: userId, partitionId: partitionId, msId: null, email: null, phoneNumber: phoneNumber, name: null, json: null, source: null, isEmailConfirmed: null).First());
            return DataContractConverters.Convert(res);
        }

        /// <summary>
        /// The update user info.
        /// </summary>
        /// <param name="userId">
        /// the user id
        /// </param>
        /// <param name="userInfo">
        /// The user to update info.
        /// </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// userToUpdate is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// user id isn't a GUID
        /// </exception>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        public User UpdateUserInfo(Guid userId, UserInfo userInfo)
        {
            string json = null;
            if (userInfo != null)
            {
                var serializerSettings = new JsonSerializerSettings
                                             {
                                                 DefaultValueHandling = DefaultValueHandling.Ignore,
                                                 Formatting = Formatting.None
                                             };
                json = JsonConvert.SerializeObject(userInfo, serializerSettings);
            }

            UserDb res = this.Execute(
                userId.ToString(),
                (context, partitionId) =>
                context.CreateOrUpdateUser(id: userId, partitionId: partitionId, msId: null, email: null, phoneNumber: null, name: null, json: json, source: null, isEmailConfirmed: null).First());
            return DataContractConverters.Convert(res);
        }

        /// <summary>
        /// Creates an unauthenticated user with the email address
        /// If the user with the email is suppressed - an error will be thrown, otherwise we return the user
        /// </summary>
        /// <param name="email">User Email address</param>
        /// <param name="source">Referrer for the user</param>
        /// <param name="userLocation">User location info from frontdoor serialized as a Base64 encoded string</param>
        /// <returns>Unauthenticated user object</returns>
        public User CreateUnauthenticatedUser(string email, string source, string userLocation = null)
        {
            User user = this.CreateOrGetUserByEmail(email, false, source, userLocation);

            return user;
        }

        /// <summary>
        ///  Runs a set of tasks to finish setting up the Unauthenticated user in UserServices
        /// </summary>
        /// <param name="userId">Unauthenticated User Id. This user has email address</param>
        public void CompleteUnauthenticatedUserSetUp(Guid userId)
        {
            User user = this.GetUserByUserId(userId);
            if (user == null)
            {
                throw new ArgumentException(string.Format("User with id {0} doesn't exist", userId));
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentException(string.Format("User with id {0} doesn't have email address", userId));
            }

            if (!user.IsEmailConfirmed)
            {
                Tuple<string, int> confirmationResponse = this.CreateConfirmationCode(user.Email, EntityType.UnAuthenticatedEmailAddress, userId);
                this.confirmationJobsQueue.Enqueue(new ConfirmationEmailCargo
                    {
                        Id = Guid.NewGuid(),
                        EntityType = EntityType.UnAuthenticatedEmailAddress,
                        EmailAddress = user.Email,
                        UserIdHash = confirmationResponse.Item1,
                        ConfirmationCode = confirmationResponse.Item2
                    });
            }
        }

        /// <summary>
        ///  Gets the merchant subscriptions by user id
        /// </summary>
        /// <param name="userId">User id of the merchant</param>
        /// <returns>List of subscriptions for the merchant user</returns>
        public IEnumerable<MerchantSubscriptionInfo> GetMerchantSubscriptionsByUserId(Guid userId)
        {
            return this.Execute(
                userId.ToString(),
                (context, partitionId) =>
                context.GetMerchantSubscriptionsByUserId(userId, partitionId)
                       .Select(DataContractConverters.Convert)
                       .ToList());
        }

        /// <summary>
        /// Updates subscription information for merchant user
        /// </summary>
        /// <param name="userId">Internal user id</param>
        /// <param name="subscriptionType">Type of subscription</param>
        /// <param name="scheduleType">Schedule for receiving merchant email</param>
        /// <param name="merchantPreferences">Settings for merchant user</param>
        public void UpdateMerchantSubscription(Guid userId, SubscriptionType subscriptionType, ScheduleType scheduleType, MerchantPreferences merchantPreferences)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None
            };
            string preferencesJson = JsonConvert.SerializeObject(merchantPreferences, serializerSettings);

            this.Execute(
                userId.ToString(),
            (context, partitionId) =>
            context.CreateOrUpdateMerchantSubscriptions(userId, partitionId, Enum.GetName(typeof(SubscriptionType), subscriptionType), Enum.GetName(typeof(ScheduleType), scheduleType), preferencesJson));
        }

        /// <summary>
        /// Unsubscribes the subscription of the merchant user
        /// </summary>
        /// <param name="userId">Merchant user id</param>
        /// <param name="subscriptionType">Type of subscription</param>
        public void DeleteMerchantSubscriptions(Guid userId, SubscriptionType subscriptionType)
        {
            this.Execute(
                userId.ToString(),
                (context, partitionId) => context.UnsubscribeMerchantSubscriptions(userId, partitionId, Enum.GetName(typeof(SubscriptionType), subscriptionType)));
        }

        /// <summary>
        /// Returns the scheduled email jobs
        /// </summary>
        /// <param name="takeCount">total jobs to return</param>
        /// <param name="continuationContext">If null - will start from the beginning of the subscriptions list
        /// Otherwise - the continuationContext should be the one that was returned in the previous call to this method</param>
        /// <returns>The EmailJobs</returns>
        public EmailJobBatchResponse GetEmailJobs(int takeCount, object continuationContext)
        {
            if (continuationContext == null)
            {
                var context = new EmailJobsBatchContext
                {
                    FromPartitionId = null,
                    FromUserId = null,
                    PartitionIndex = 0,
                    HasMore = true
                };
                return this.GetNextEmailJobsBatch(takeCount, context);
            }

            if (!(continuationContext is EmailJobsBatchContext))
            {
                throw new ArgumentException(
                    "Continuation Context must be null or the previous one", "continuationContext");
            }

            return this.GetNextEmailJobsBatch(takeCount, (EmailJobsBatchContext)continuationContext);
        }

        /// <summary>
        /// Updates the specified email job by setting the next execution time for the job
        /// </summary>
        /// <param name="jobId">Job Id</param>
        public void UpdateEmailJob(Guid jobId)
        {
            this.Execute(context => context.UpdateEmailJob(jobId));
        }

        /// <summary>
        /// Returns the  scheduled offers email jobs
        /// </summary>
        /// <returns>Offers Email Schedule</returns>
        public List<OffersEmailSchedule> GetNextScheduledEmails()
        {
            List<OffersEmailSchedule> lstOffersEmailSchedule = null;
            var scheduledEmails = this.Execute(context => context.GetScheduledEmails().ToList());
            if (scheduledEmails != null)
            {
                lstOffersEmailSchedule = new List<OffersEmailSchedule>();
                foreach (var scheduledEmail in scheduledEmails)
                {
                    OffersEmailSchedule offersEmailSchedule = new OffersEmailSchedule
                        {
                            CampaignName = scheduledEmail.CampaignName,
                            ScheduleId = scheduledEmail.ScheduleId,
                            IsCloUserTargeted = scheduledEmail.IsClo ?? false,
                            IsTestEmail = scheduledEmail.IsTest ?? true,
                            MetaData = scheduledEmail.Metadata,
                            TemplateUrl = scheduledEmail.Url
                        };
                    lstOffersEmailSchedule.Add(offersEmailSchedule);
                }
            }

            return lstOffersEmailSchedule;
        }

        /// <summary>
        ///  Updates the specified offers email job
        /// </summary>
        /// <param name="scheduleId">Schedule Id</param>
        /// <param name="campaignName">Name of the email campaign</param>
        /// <param name="status">Status of the job</param>
        public void UpdateEmailSchedule(int scheduleId, string campaignName, string status)
        {
            this.Execute(context => context.UpdateEmailSchedule(scheduleId, campaignName, status));
        }

        #endregion

        #region Methods

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        private EmailsSubscriptionsBatchResponse BuildEmailSubscriptionsBatchResponse(
            IList<GetEmailSubscriptions_Result> emailSubscriptionsResults,
            EmailSubscriptionsBatchContext currentCallContext,
            int callTakeMaxCount)
        {
            var nextCallContext = new EmailSubscriptionsBatchContext
                                      {
                                          HasMore = true
                                      };

            // If we reached the end of this partition
            if (emailSubscriptionsResults.Count() < callTakeMaxCount)
            {
                nextCallContext.HasMore = false;
            }
            else
            {
                // We still have more elements in the current partition - continue from the same partition in the next call
                nextCallContext.PartitionIndex = currentCallContext.PartitionIndex;
                GetEmailSubscriptions_Result lastRecord = emailSubscriptionsResults.Last();
                nextCallContext.FromLocationId = lastRecord.LocationId;
                nextCallContext.FromPartitionId = lastRecord.PartitionId;
                nextCallContext.FromUserId = lastRecord.UserId;
            }

            return new EmailsSubscriptionsBatchResponse
                       {
                           EmailSubscriptions =
                               emailSubscriptionsResults.Select(
                                   DataContractConverters.Convert).ToList(),
                           ContinuationContext = nextCallContext,
                           HasMore = nextCallContext.HasMore
                       };
        }

        /// <summary>
        /// Returns the next set of email jobs
        /// </summary>
        /// <param name="takeCount">total jobs to return</param>
        /// <param name="continuationContext">continuation context to mark the set of jobs to return</param>
        /// <returns>set of email jobs</returns>
        private EmailJobBatchResponse GetNextEmailJobsBatch(int takeCount, EmailJobsBatchContext continuationContext)
        {
            if (!continuationContext.HasMore)
            {
                throw new InvalidOperationException("End of list was reached in the previous call");
            }

            List<GetEmailJobs_Result> emailJobs =
                this.Execute(
                    context =>
                    context.GetEmailJobs(
                        takeCount,
                        continuationContext.FromPartitionId,
                        continuationContext.FromUserId).ToList());

            return this.BuildEmailJobsBatchResponse(emailJobs, continuationContext, takeCount);
        }

        /// <summary>
        /// Returns the email jobs response
        /// </summary>
        /// <param name="emailJobsResults">the email jobs result</param>
        /// <param name="currentCallContext">current call context</param>
        /// <param name="callTakeMaxCount">max records to return</param>
        /// <returns>the emailjobs</returns>
        private EmailJobBatchResponse BuildEmailJobsBatchResponse(
           IList<GetEmailJobs_Result> emailJobsResults,
           EmailJobsBatchContext currentCallContext,
           int callTakeMaxCount)
        {
            var nextCallContext = new EmailJobsBatchContext()
            {
                HasMore = true
            };

            // If we reached the end of this partition
            if (emailJobsResults.Count() < callTakeMaxCount)
            {
                nextCallContext.HasMore = false;
            }
            else
            {
                // We still have more elements in the current partition - continue from the same partition in the next call
                nextCallContext.PartitionIndex = currentCallContext.PartitionIndex;
                GetEmailJobs_Result lastRecord = emailJobsResults.Last();
                nextCallContext.FromPartitionId = lastRecord.PartitionId;
                nextCallContext.FromUserId = lastRecord.UserId;
            }

            return new EmailJobBatchResponse
            {
                EmailJobs = emailJobsResults.Select(DataContractConverters.Convert).ToList(),
                ContinuationContext = nextCallContext,
                HasMore = nextCallContext.HasMore
            };
        }

        /// <summary>
        /// The change user's database layer records  email.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="newEmail">
        /// The new email.
        /// </param>
        /// <param name="isEmailConfirmed">if true - this method is being called in the context where the email address was confirmed. </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        private User ChangeUserDbEmail(User user, string newEmail, bool isEmailConfirmed)
        {
            // If the current email of the user isn't null - delete it from external users table
            if (!string.IsNullOrEmpty(user.Email))
            {
                this.Execute(
                    user.Email,
                    (context, partitionId) =>
                    context.DeleteExternalUser(user.Email, partitionId, (int)UserExternalIdType.Email));
            }

            this.Execute(
                newEmail,
                (context, partitionId) =>
                context.CreateNotExistingExternalUser(user.Id, newEmail, partitionId, (int)UserExternalIdType.Email));

            // Update email address
            UserDb res = this.Execute(
                user.Id.ToString(),
                (context, partitionId) =>
                context.CreateOrUpdateUser(id: user.Id, partitionId: partitionId, msId: null, email: newEmail, phoneNumber: null, name: null, json: null, source: null, isEmailConfirmed: isEmailConfirmed).First());
            return DataContractConverters.Convert(res);
        }

        /// <summary>
        /// If target user doesn't have any subscription 
        /// The subscription of the source user will be copied to the target user
        /// </summary>
        /// <param name="sourceUser">
        /// The source user
        /// </param>
        /// <param name="targetUser">
        /// the target user
        /// </param>
        private void CopyUserSubscriptions(User sourceUser, User targetUser)
        {
            var targetUserSubscriptions =
                this.GetEmailSubscriptionsByUserId(targetUser.Id, true);
            if (targetUserSubscriptions == null || !targetUserSubscriptions.Any())
            {
                var sourceUserSubscription = this.GetEmailSubscriptionsByUserId(sourceUser.Id, true);
                if (sourceUserSubscription != null && sourceUserSubscription.Any())
                {
                    if (sourceUserSubscription.Any(elem => elem.SubscriptionType == SubscriptionType.WeeklyDeals))
                    {
                        this.UpdateEmailSubscriptions(targetUser.Id, sourceUserSubscription.Select(elem => elem.LocationId), SubscriptionType.WeeklyDeals);
                    }

                    if (sourceUserSubscription.Any(elem => elem.SubscriptionType == SubscriptionType.Promotional))
                    {
                        this.UpdateEmailSubscriptions(targetUser.Id, sourceUserSubscription.Select(elem => elem.LocationId), SubscriptionType.Promotional);
                    }
                }
            }
        }

        /// <summary>
        ///     The get context.
        /// </summary>
        /// <returns>
        ///     The <see cref="UsersEntities" />.
        /// </returns>
        private UsersEntities CreateDbContext()
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                return new UsersEntities();
            }

            return new UsersEntities(this.connectionString);
        }

        /// <summary>
        /// Delete email based user from the system.
        /// </summary>
        /// <param name="user">
        /// The user to delete.
        /// </param>
        private void DeleteEmailUser(User user)
        {
            this.Execute(
                user.Email,
                (context, partitionId) =>
                context.DeleteExternalUser(user.Email, partitionId, (int)UserExternalIdType.Email));

            // Delete the old user
            this.Execute(
                user.Id.ToString(), (context, partitionId) => context.DeleteUser(user.Id, partitionId));
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        private TResult Execute<TResult>(string id, Func<UsersEntities, int, TResult> func)
        {
            return this.retryPolicy.ExecuteAction(
                () =>
                {
                    using (UsersEntities context = this.CreateDbContext())
                    {
                        ((IObjectContextAdapter)context).ObjectContext.Connection.Open();
                        int partitionId = this.partitioner.PartitionId(id);
                        return func(context, partitionId);
                    }
                });
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here.")]
        private TResult Execute<TResult>(Func<UsersEntities, TResult> func)
        {
            return this.retryPolicy.ExecuteAction(
                () =>
                {
                    using (UsersEntities context = this.CreateDbContext())
                    {
                        ((IObjectContextAdapter)context).ObjectContext.Connection.Open();
                        return func(context);
                    }
                });
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Reviewed. Suppression is OK here. View Public method for documentation")]
        private EmailsSubscriptionsBatchResponse GetNextEmailSubscriptionsBatch(
            int takeCount, bool? isActive, EmailSubscriptionsBatchContext continuationContext, SubscriptionType subscriptionType)
        {
            if (!continuationContext.HasMore)
            {
                throw new InvalidOperationException("End of list was reached in the previous call");
            }

            List<GetEmailSubscriptions_Result> emailSubscriptions =
                this.Execute(
                    context =>
                    context.GetEmailSubscriptions(
                        takeCount,
                        isActive,
                        continuationContext.FromPartitionId,
                        continuationContext.FromUserId,
                        continuationContext.FromLocationId,
                        subscriptionType.ToString()).ToList());

            return this.BuildEmailSubscriptionsBatchResponse(
                emailSubscriptions, continuationContext, takeCount);
        }

        /// <summary>
        /// The add subscription list parameter.
        /// </summary>
        /// <param name="sqlCommand">
        /// The sql command.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <param name="locationIds">
        /// The location ids.
        /// </param>
        private void AddSubscriptionListParameter(DbCommand sqlCommand, string parameterName, IEnumerable<string> locationIds)
        {
            const string TableTypeName = "[dbo].[SubscriptionListType]";

            const string LocationIdColName = "LocationId";
            const string IsActiveColName = "IsActive";

            var dataTable = new DataTable();

            // we create column names as per the type in DB 
            dataTable.Columns.Add(LocationIdColName, typeof(string));
            dataTable.Columns.Add(IsActiveColName, typeof(bool));

            foreach (var locationId in locationIds)
            {
                DataRow row = dataTable.NewRow();
                row.SetField(LocationIdColName, locationId);
                row.SetField(IsActiveColName, true);
                dataTable.Rows.Add(row);
            }

            SqlParameter p = new SqlParameter(parameterName, SqlDbType.Structured) { TypeName = TableTypeName, Value = dataTable };
            sqlCommand.Parameters.Add(p);
            p.TypeName = TableTypeName;
            p.Value = dataTable;
        }

        /// <summary>
        /// if the user with the given microsoft id doesn't exist, it will be created.
        ///     The user object for the newly created user or the already existing user will be returned.
        /// </summary>
        /// <param name="microsoftId">
        /// The user's microsoft id.
        /// </param>
        /// <param name="userSource">
        /// Source of the email address
        /// </param>
        /// <param name="userName">
        /// The user's name.
        /// </param>
        /// <param name="inputUserId">
        /// The user id (optional)
        /// </param>
        /// <param name="userLocation">User location info from frontdoor serialized as a Base64 encoded string</param>
        /// <returns>
        /// The <see cref="User"/>. object
        /// </returns>
        /// <exception cref="EntityException">
        /// Any Entities framework related error
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <param name="microsoftId">
        /// </param>
        /// is null or emtpy string
        /// </exception>
        private User CreateOrGetUserByMsIdFromDb(string microsoftId, string userSource = null, string userName = null,
            Guid? inputUserId = null, UserLocation userLocation = null)
        {
            string userInfoJson = null;
            if (userLocation != null)
            {
                userInfoJson = JsonConvert.SerializeObject(userLocation);
            }

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(userSource))
            {
                sb.Append(userSource);
            }

            if (!string.IsNullOrEmpty(userInfoJson))
            {
                if (string.IsNullOrEmpty(userSource) || !userSource.Contains("#"))
                {
                    sb.Append("##");
                }
                else
                {
                    sb.Append("#");
                }

                sb.Append(userInfoJson);
            }

            if (sb.Length > 0)
            {
                userSource = sb.ToString();
            }

            Guid userId = this.Execute(
                microsoftId,
                (context, partitionId) =>
                {
                    ObjectResult<ExternalUserDb> extUsers = context.CreateNotExistingExternalUser(
                        inputUserId, microsoftId, partitionId, (int) UserExternalIdType.MsId);
                    ExternalUserDb extUser = extUsers.First();
                    return extUser.UserId;
                });

            
            User user = GetUserByUserId(userId);

            //If user doesn't exist (new user)
            if (user == null)
            {
                //set their transaction notification preference to Email and phone by default
                UserInfo userInfo = new UserInfo
                {
                    Preferences = new UserPreferences
                    {
                        TransactionNotificationMedium = TransactionNotificationPreference.Email |
                                                        TransactionNotificationPreference.Phone
                    }
                };
                var serializerSettings = new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Formatting = Formatting.None
                };

                string json = JsonConvert.SerializeObject(userInfo, serializerSettings);

                // If the user doesn't exists it will be created with the related microsoftId.
                UserDb res = this.Execute(
                    userId.ToString(),
                    (context, partitionId) =>
                    context.CreateOrUpdateUser(id: userId, partitionId: partitionId, msId: microsoftId, email: null, phoneNumber: null, name: userName, json: json, source: userSource, isEmailConfirmed: null).First());
                return DataContractConverters.Convert(res);
            }

            return user;
        }

        #endregion
    }
}