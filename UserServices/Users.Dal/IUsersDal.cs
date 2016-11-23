//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The UsersDal interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Users.Dal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Users.Dal.DataModel;

    /// <summary>
    /// The UsersDal interface.
    /// </summary>
    public interface IUsersDal
    {
        /// <summary>
        /// if the user with the given address doesn't exist, it will be created.
        /// The user object for the newly created user or the already exists user will be returned.
        /// </summary>
        /// <param name="email"> The email address. </param>
        /// <param name="isEmailConfirmed">if true - this method is being called in the context where the email address was confirmed. </param>
        /// <param name="userSource"> Source of the email address </param>
        /// <param name="userLocation">User location info from frontdoor serialized as a Base64 encoded string</param>
        /// <returns> The <see cref="User"/>. object </returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        /// <exception cref="ArgumentException"><param name="email"/> is null or emtpy string</exception>
        User CreateOrGetUserByEmail(string email, bool isEmailConfirmed, string userSource = null, string userLocation = null);

        /// <summary>
        /// if the user with the given microsoft id doesn't exist, it will be created.
        /// The user object for the newly created user or the already existing user will be returned.
        /// </summary>
        /// <param name="microsoftId"> The user's microsoft id.
        /// </param>
        /// <param name="userSource">
        /// Source of the email address
        /// </param>
        /// <param name="userName">
        /// User's name
        /// </param>
        /// <param name="userId">the user id (optional) </param>
        /// <param name="userLocation">User location info from frontdoor serialized as a Base64 encoded string</param>
        /// <returns> The <see cref="User"/>. object </returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        /// <exception cref="ArgumentException"><param name="microsoftId"/> is null or emtpy string</exception>
        User CreateOrGetUserByMsId(string microsoftId, string userSource = null, string userName = null, Guid? userId = null, UserLocation userLocation = null);

        /// <summary>
        /// Update the user's email address
        /// </summary>
        /// <remarks>
        /// -If the user (the target user) is a suppressed user or doesn't have an associated microsoft id the operation isn't allowed and an exception will be thrown.
        /// -If a user with the same email address (the source user) already exists an attempt to merge the users will be done:
        ///     -If the source user has an associated microsoft id or is suppressed user the operation isn't allowed and an exception will be thrown
        ///     -otherwise the subscriptions of the source user will be attached to the subscriptions of the target user and the source will be deleted from the system (only if the target user doesn't have subscriptions)
        /// </remarks>
        /// <param name="userId"> The id of the target user. </param>
        /// <param name="email"> The new email address. </param>
        /// <param name="isEmailConfirmed">if true - this method is being called in the context where the email address was confirmed. </param>
        /// <returns>
        /// The <see cref="User"/>.
        /// </returns>
        /// <exception cref="ArgumentException"> email is null or empty </exception>
        /// <exception cref="UserNotExistsException"> the target user doesn't exist in the system </exception>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        User UpdateUserEmail(Guid userId, string email, bool isEmailConfirmed);

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
        User UpdateUserPhoneNumber(Guid userId, string phoneNumber);

        /// <summary>
        /// The update user info. </summary>
        /// <param name="userId"> the user id </param>
        /// <param name="userInfo"> The user to update info. </param>
        /// <returns> The <see cref="User"/>.
        /// </returns> 
        /// <exception cref="ArgumentNullException"> userToUpdate is null  </exception>
        /// <exception cref="ArgumentException"> user id isn't a GUID  </exception>
        /// <exception cref="EntityException"> Any Entities framework related error </exception>
        User UpdateUserInfo(Guid userId, UserInfo userInfo);


        /// <summary>
        /// Get the confirm entity
        /// </summary>
        /// <param name="userIdHash"> the user id hash
        /// </param><param name="entityType">the entity type  </param>
        /// <returns> The <see cref="string"/>the confirm entity </returns>
        ConfirmEntity GetConfirmationEntity(string userIdHash, EntityType entityType);

        /// <summary>
        /// The confirm entity.
        /// </summary>
        /// <param name="userIdHash"> The user Id hash.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="confirmationCode"> The confirmation code.
        /// </param>
        /// <returns>
        /// The <see cref="ConfirmEntityResult"/>.
        /// </returns>
        ConfirmEntityResult ConfirmEntity(string userIdHash, EntityType entityType, int confirmationCode);

        /// <summary> The create confirmation code - simplified  </summary>
        /// <param name="user"> The user object.
        /// </param>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="maxRetryCount"> The max retry count. if not specified the default value will be used
        /// </param>
        /// <param name="codeExpirationWindow"> The code expiration window. If not specified the default value will be used 
        /// </param>
        /// <returns> A tuple of user id hash and a confirmation code  </returns>
        Tuple<string, int> CreateConfirmationCode(User user, EntityType entityType, int? maxRetryCount = null, TimeSpan? codeExpirationWindow = null);

        /// <summary>
        /// create confirmation code.
        /// </summary>
        /// <param name="entity"> The entity.  </param>
        /// <param name="entityType"> The entity type.  </param>
        /// <param name="userId"> The user id.  </param>
        /// <param name="maxRetryCount"> The max retry count. if not specified the default value will be used
        /// </param>
        /// <param name="codeExpirationWindow"> The code expiration window. If not specified the default value will be used 
        /// </param>
        /// <returns> A tuple of user id hash and a confirmation code  </returns>
        Tuple<string, int> CreateConfirmationCode(string entity, EntityType entityType, Guid userId, int? maxRetryCount = null, TimeSpan? codeExpirationWindow = null);

        /// <summary> The log user confirm email resend. </summary>
        /// <param name="userId"> The user id. </param>
        /// <param name="entityType"> The entity type. </param>
        void LogUserConfirmEmailResend(Guid userId, EntityType entityType);

        /// <summary> The get user confirm email resend count. </summary>
        /// <param name="userId"> The user id. </param>
        /// <param name="entityType"> The entity type. </param>
        /// <param name="fromDateTime"> The from date time. </param>
        /// <returns> The number of times confirm email resend was executed for user with user.
        /// The <see cref="int"/>.
        /// </returns>
        int GetUserConfirmEmailResendCount(Guid userId, EntityType entityType, DateTime fromDateTime);

        /// <summary>
        /// Suppress the user from the given email. This is a one way operation.
        /// A suppressed user won't appear in the different query results
        /// </summary>
        /// <param name="email">the email to suppress</param>
        /// <param name="isSuppressed">The user is suppressed or unsuppressed</param>
        /// <returns>the suppressed user object</returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        User UpdateUserSuppressInfo(string email, bool isSuppressed);

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
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        User GetUserByExternalId(string externalId, UserExternalIdType externalIdType);

        /// <summary>
        /// gets user by internal id
        /// </summary>
        /// <param name="id">internal user id</param>
        /// <returns>
        /// the user object if exists, otherwise returns null
        /// </returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        User GetUserByUserId(Guid id);

        /// <summary>
        /// Updates email subscriptions. Will override any existing subscription of the user.
        /// </summary>
        /// <param name="userId">
        /// The user id 
        /// </param>
        /// <param name="locationIds">
        /// the email subscriptions location ids
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        void UpdateEmailSubscriptions(Guid userId, IEnumerable<string> locationIds, SubscriptionType subscriptionType);

        /// <summary>
        /// Create or update email subscription
        /// </summary>
        /// <param name="createOrUpdate">
        /// The create Or Update.
        /// </param>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        void CreateOrUpdateEmailSubscription(EmailSubscription createOrUpdate);

        /// <summary>
        /// Deletes email subscription
        /// </summary>
        /// <param name="userId">internal user id</param>
        /// /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <param name="locationId">the location id. if null, all the email subscriptions of this user will be deleted </param>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        void DeleteEmailSubscriptions(Guid userId, SubscriptionType subscriptionType, string locationId = null);

        /// <summary>
        /// Gets email subscriptions by user id
        /// </summary>
        /// <param name="userId">internal user id</param>
        /// <param name="isActive">
        /// isActive = true: returns only active subscriptions 
        /// isActive = false: returns only disabled subscriptions
        /// isActive = null: returns all the subscriptions
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <returns>list of email subscriptions</returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        IEnumerable<EmailSubscription> GetEmailSubscriptionsByUserId(Guid userId, bool? isActive, string subscriptionType = null);

        /// <summary>
        /// Gets email subscriptions by location id
        /// </summary>
        /// <param name="locationId">the location id</param>
        /// <param name="isActive">
        /// isActive = true: returns only active subscriptions 
        /// isActive = false: returns only disabled subscriptions
        /// isActive = null: returns all the subscriptions
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <returns>list of email subscriptions</returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        IEnumerable<EmailSubscription> GetSubscriptionsByLocationId(string locationId, bool? isActive, SubscriptionType subscriptionType);

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
        IEnumerable<EmailSubscription> GetEmailSubscriptionsByEmail(string emailAddress, bool? isActive, string subscriptionType = null);

        /// <summary>
        /// Gets subscriptions in batches
        /// </summary>
        /// <param name="takeCount">
        /// the maximum number of elements that will be returned in this batch request
        /// </param>
        /// <param name="isActive">
        /// isActive = true: returns only active subscriptions 
        /// isActive = false: returns only disabled subscriptions
        /// isActive = null: returns all the subscriptions
        /// </param>
        /// <param name="continuationContext">
        /// If null - will start from the beginning of the subscriptions list
        /// Otherwise - the continuationContext should be the one that was returned in the previous call to this method
        /// </param>
        /// <param name="subscriptionType">
        /// Identifies the type of email subscription (WeeklyDeals, Promotional email etc) for this user 
        /// </param>
        /// <returns>
        /// subscriptions batch response. The response includes:
        /// - List of subscriptions
        /// - Continuation Context object that should be passed in the next call for this method
        /// - Parameter that indicates if there are more subscriptions 
        /// </returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        /// <exception cref="ArgumentException"><param name="continuationContext"/> is illegel</exception>
        /// <exception cref="InvalidOperationException">The operation is invald. This can happen if a lot of time passed between the subsequent batch requests or the end of list reached in a previous call</exception>
        EmailsSubscriptionsBatchResponse GetNextEmailSubscriptionsBatch(int takeCount, bool? isActive, object continuationContext, SubscriptionType subscriptionType);

        /// <summary>
        /// Get unsubscribe url information of specific url
        /// </summary>
        /// <param name="userId">
        /// The user internal id.
        /// </param>
        /// <returns>Unsubscribe information</returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        EmailUnsubscribeInfo GetUnsubscribeUrlInfo(Guid userId);

        /// <summary>
        /// Get a list unsubscribe url information to update. Will return entities with unsubscribe url equals to null or entities with unsubscribe url 
        /// that were last updated before <paramref name="updatedBefore"/>
        /// </summary>
        /// <param name="updatedBefore"> will return entities that were last updated before this time  </param>
        /// <returns>Email unsubscribe information list</returns>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        IEnumerable<EmailUnsubscribeInfo> GetUnsubscribeInfoToUpdate(DateTime updatedBefore);

        /// <summary>
        /// Creates or updates unsubscribe url
        /// </summary>
        /// <param name="userId"> internal user id </param>
        /// <param name="unsubscribeUrl"> the unsubscribe url to assign </param>
        /// <exception cref="EntityException">Any Entities framework related error</exception>
        void CreateOrUpdateUnsubscribeUrl(Guid userId, string unsubscribeUrl);

        /// <summary>
        /// Creates an unauthenticated user with the email address
        /// If the user with the email is suppressed - an error will be thrown, otherwise we return the user
        /// </summary>
        /// <param name="email">User Email address</param>
        /// <param name="source">Referrer for the user</param>
        /// <param name="userLocation">User location info from frontdoor serialized as a Base64 encoded string</param>
        /// <returns>Unauthenticated user object</returns>
        User CreateUnauthenticatedUser(string email, string source, string userLocation = null);

        /// <summary>
        /// Runs a set of tasks to finish setting up the Unauthenticated user in UserServices
        /// </summary>
        /// <param name="userId">Unauthenticated User Id. This user has email address</param>
        void CompleteUnauthenticatedUserSetUp(Guid userId);

        /// <summary>
        /// Gets the merchant subscriptions by user id
        /// </summary>
        /// <param name="userId">User id of the merchant</param>
        /// <returns>List of subscriptions for the merchant user</returns>
        IEnumerable<MerchantSubscriptionInfo> GetMerchantSubscriptionsByUserId(Guid userId);

        /// <summary>
        /// Updates subscription information for merchant user
        /// </summary>
        /// <param name="userId">Internal user id</param>
        /// <param name="subscriptionType">Type of subscription</param>
        /// <param name="scheduleType">Schedule for receiving merchant email</param>
        /// <param name="merchantPreferences">Settings for merchant user</param>
        void UpdateMerchantSubscription(Guid userId, SubscriptionType subscriptionType, ScheduleType scheduleType, MerchantPreferences merchantPreferences);

        /// <summary>
        /// Unsubscribes the subscription of the merchant user
        /// </summary>
        /// <param name="userId">Merchant user id</param>
        /// <param name="subscriptionType">Type of subscription</param>
        void DeleteMerchantSubscriptions(Guid userId, SubscriptionType subscriptionType);

        /// <summary>
        /// Returns the scheduled email jobs
        /// </summary>
        /// <param name="takeCount">total jobs to return</param>
        /// <param name="continuationContext">If null - will start from the beginning of the subscriptions list
        /// Otherwise - the continuationContext should be the one that was returned in the previous call to this method</param>
        /// <returns>The EmailJobs</returns>
        EmailJobBatchResponse GetEmailJobs(int takeCount, object continuationContext);

        /// <summary>
        /// Updates the specified email job by setting the next execution time for the job
        /// </summary>
        /// <param name="jobId">Job Id</param>
        void UpdateEmailJob(Guid jobId);

        /// <summary>
        /// Returns the  scheduled offers email jobs
        /// </summary>
        /// <returns>Offers Email Schedule</returns>
        List<OffersEmailSchedule> GetNextScheduledEmails();

        /// <summary>
        ///  Updates the specified offers email job
        /// </summary>
        /// <param name="scheduleId">Schedule Id</param>
        /// <param name="campaignName">Name of the email campaign</param>
        /// <param name="status">Status of the job</param>
        void UpdateEmailSchedule(int scheduleId, string campaignName, string status);
    }
}
