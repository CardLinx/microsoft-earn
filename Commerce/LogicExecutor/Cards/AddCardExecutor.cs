//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logic
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;
    using Lomo.Commerce.CardLink;
    using Lomo.Commerce.Configuration;
    using Lomo.Commerce.DataAccess;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.DataModels;
    using Lomo.Commerce.Context;
    using Lomo.Commerce.Utilities;
    using Lomo.Commerce.Service;
    using Lomo.Commerce.DataContracts;
    using System.Collections.Generic;
    using Users.Dal;
    using System.Net.Mail;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Contains logic necessary to execute an add card request.
    /// </summary>
    public class AddCardExecutor
    {
        /// <summary>
        /// Initializes a new instance of the AddCardExecutor class.
        /// </summary>
        /// <param name="context">
        /// The context to use while processing the request.
        /// </param>
        public AddCardExecutor(CommerceContext context)
        {
            Context = context;
            SharedUserLogic = new SharedUserLogic(Context, CommerceOperationsFactory.UserOperations(Context));
        }

        /// <summary>
        /// Executes the Add card invocation.
        /// </summary>
        public async Task Execute()
        {
            try
            {
                AddCardConcluder addCardConcluder = new AddCardConcluder(Context);

                if ((Context[Key.NewCardInfo] as NewCardInfo) != null)
                {
                    if (CardMayBeValid() == true && CardProviderRejected() == false)
                    {
                        ResultCode resultCode = PlaceUserInContext();
                        if (resultCode == ResultCode.Success)
                        {
                            User user = Context[Key.User] as User;
                            if (user != null)
                            {
                                PopulateContextUserAndCard();
                                Context.Log.Verbose("Attempting to add the card for the user to all current partners.");
                                AddCardInvoker addCardInvoker = new AddCardInvoker(Context);
                                await addCardInvoker.Invoke();
                            }
                            else
                            {
                                addCardConcluder.Conclude(ResultCode.UnexpectedUnregisteredUser);
                            }
                        }
                        else
                        {
                            addCardConcluder.Conclude(resultCode);
                        }
                    }
                    else
                    {
                        addCardConcluder.Conclude(ResultCode.InvalidCard);
                    }
                }
                else
                {
                    addCardConcluder.Conclude(ResultCode.ParameterCannotBeNull);
                }
            }
            catch (Exception ex)
            {
                ((ResultSummary)Context[Key.ResultSummary]).SetResultCode(ResultCode.UnknownError);
                RestResponder.BuildAsynchronousResponse(Context, ex);
            }
        }

        /// <summary>
        /// Performs preliminary checks to determine whether the card may be valid.
        /// </summary>
        /// <returns>
        /// * True if the card may be valid.
        /// * Else returns false.
        /// </returns>
        /// <remarks>
        /// This method does not determine whether the card actually _is_ valid, i.e. whether it can be used to make
        /// purchases, belongs to the specified person, etc.. Instead, it determines whether the card _may be_ valid, by
        /// ensuring the card brand matches the card number, that the stated expiration date is not in the past, and that the
        /// card number is valid according to Luhn's modulus 10 algorithm.
        /// </remarks>
        internal bool CardMayBeValid()
        {
            bool result = false;

            Context.Log.Verbose("Determining if NewCardInfo object may be valid.");

            // Ensure the card number matches the card brand.
            NewCardInfo newCardInfo = (NewCardInfo)Context[Key.NewCardInfo];
            if (String.IsNullOrWhiteSpace(newCardInfo.Number) == false && (newCardInfo.Number[0] - '0') == (int)ParseCardBrand(newCardInfo.CardBrand))
            {
                // Get baseline DateTime for the last day of the current month.
                DateTime currentDate = DateTime.UtcNow;
                int year = currentDate.Year;
                int month = currentDate.Month;
                int lastDayOfMonth = DateTime.DaysInMonth(year, month);
                DateTime endOfCurrentMonth = new DateTime(year, month, lastDayOfMonth);

                // Get baseline DateTime for the last day of the month in which the card expires.
                year = newCardInfo.Expiration.Year;
                month = newCardInfo.Expiration.Month;
                lastDayOfMonth = DateTime.DaysInMonth(year, month);
                DateTime endOfExpirationMonth = new DateTime(year, month, lastDayOfMonth);

                // If the card has not expired, continue validation.
                if (endOfExpirationMonth >= endOfCurrentMonth)
                {
                    // Validate card number using Luhn's algorithm.
                    // Luhn's algorithm works by starting from the last (check) digit in the credit card number and then, for the
                    // check digit and every second previous digit, adding those digits into a total, and, for all other
                    // digits, multiplying those digits by 2, adding the resulting digits together, and adding those sums into
                    // the total. Those modified values are determinant, and can therefore be represented in an array for fast
                    // runtime lookups.
                    int checksum = 0;
                    bool modifyDigits = false;
                    bool nonDigitFound = false;
                    for (int count = newCardInfo.Number.Length - 1; count >= 0; count--)
                    {
                        int digit = newCardInfo.Number[count] - '0';
                        if (digit < 0 || digit > 9)
                        {
                            nonDigitFound = true;
                            break;
                        }

                        if (modifyDigits == false)
                        {
                            checksum += digit;
                        }
                        else
                        {
                            checksum += LuhnModifiedValues[digit];
                        }

                        modifyDigits = !modifyDigits;
                    }

                    if (nonDigitFound == false)
                    {
                        result = checksum % 10 == 0;
                    }
                }
            }

            if (result == true)
            {
                Context.Log.Verbose("NewCardInfo may be valid.");
            }
            else
            {
                Context.Log.Verbose("NewCardInfo cannot be valid.");
            }

            return result;
        }


        /// <summary>
        /// Checks whether Card number matches the rejection list.
        /// </summary>
        /// <returns>
        /// * True if the card matches rejection list.
        /// * Else returns false.
        /// </returns>
        /// <remarks>
        /// This method takes configurable rejection list from Configuration and checks the card number against it.
        /// </remarks>
        internal bool CardProviderRejected()
        {
            bool result = false;
            if(ListCardProviderRejectionMask == null )
            {
                string rejectMask = CommerceServiceConfig.Instance.CardProviderRejectionMask;
                ListCardProviderRejectionMask = rejectMask.Split(';');
            }

            NewCardInfo newCardInfo = (NewCardInfo)Context[Key.NewCardInfo];
            string cardNumber = newCardInfo.Number;
           
            foreach(string regEx in ListCardProviderRejectionMask)
            {
                if (regEx.Length > 0)
                {
                    if (Regex.IsMatch(cardNumber, regEx) == true)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets or sets the context for the API being invoked.
        /// </summary>
        internal CommerceContext Context { get; set; }

        /// <summary>
        /// Gets ors sets the object through which shared user logic can be executed.
        /// </summary>
        internal SharedUserLogic SharedUserLogic { get; set; }

        /// <summary>
        /// Gets or sets the data access object to use for User operations.
        /// </summary>
        internal IUserOperations UserOperations { get; set; }

        /// <summary>
        /// Places the User object representing the person making this request to the context.
        /// </summary>
        /// <returns>
        /// The ResultCode corresponding to the result of the operation.
        /// </returns>
        /// <remarks>
        /// If flagged to do so, a user account will be created and associated with the specified e-mail address, if the e-mail
        /// address has not already been used.
        /// </remarks>
        private ResultCode PlaceUserInContext()
        {
            ResultCode result = ResultCode.Success;

            bool createUnauthenticatedAccount = false;
            if (Context[Key.CreateUnauthenticatedAccount] != null)
            {
                createUnauthenticatedAccount = (bool)Context[Key.CreateUnauthenticatedAccount];
            }

            if (createUnauthenticatedAccount == true)
            {
                string emailAddress = Context[Key.EmailAddress] as string;
                if (String.IsNullOrWhiteSpace(emailAddress) == false)
                {
                    try
                    {
                        // Ensure the e-mail address may be valid.
                        MailAddress mailAddress = new MailAddress(emailAddress);

                        // Attempt to add a user to User Services via Users Dal and obtain its authentication vector.
                        IUsersDal usersDal = PartnerFactory.UsersDal(Context.Config);
                        Users.Dal.DataModel.User fullUser = usersDal.CreateUnauthenticatedUser(mailAddress.Address, (string)Context[Key.ReferrerId],
                                                                                               (string)Context[Key.UserLocation]);
                        UnauthenticatedAddCardResponse response = (UnauthenticatedAddCardResponse)Context[Key.Response];
                        if (String.IsNullOrWhiteSpace(fullUser.MsId) == true)
                        {
                            response.AuthenticationVector = AuthenticationVector.Email.ToString();
                        }
                        else if (fullUser.MsId.StartsWith("FB-", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            response.AuthenticationVector = AuthenticationVector.Facebook.ToString();
                        }
                        else
                        {
                            response.AuthenticationVector = AuthenticationVector.MicrosoftAccount.ToString();
                        }

                        Guid userId = fullUser.Id;
                        Context[Key.GlobalUserId] = userId;

                        // If the user returned by User Services has not already been registered in the Commerce system, register a new Commerce user.
                        User user = SharedUserLogic.RetrieveUser();
                        if (user == null)
                        {
                            user = new User(userId, Guid.NewGuid());
                            Context[Key.User] = user;
                            result = SharedUserLogic.AddUser();

                            if (result == ResultCode.Created)
                            {
                                Analytics.AddRegisterUserEvent(user.GlobalId, user.AnalyticsEventId, Guid.Empty, Context[Key.ReferrerId] as string);
                                result = ResultCode.Success;
                            }
                        }
                        else
                        {
                            Context[Key.User] = user;
                        }

                        // If the user was added or retrieved successfully, proceed.
                        if (result == ResultCode.Success)
                        {
                            // If the user has not already signed up officially with Bing Offers, proceed.
                            if (response.AuthenticationVector == AuthenticationVector.Email.ToString())
                            {
                                // If the user has not already added a card, proceed.
                                SharedCardLogic sharedCardLogic = new SharedCardLogic(Context, CommerceOperationsFactory.CardOperations(Context));
                                if (sharedCardLogic.RetrieveUserCards().Count() == 0)
                                {
                                    response.ActivationToken = fullUser.ActivationToken;
                                }
                                else
                                {
                                    result = ResultCode.UnauthenticatedUserAlreadyExists;
                                }
                            }
                            else
                            {
                                result = ResultCode.UserAlreadyExists;
                            }
                        }
                    }
                    catch(FormatException)
                    {
                        result = ResultCode.InvalidParameter;
                    }
                }
                else
                {
                    result = ResultCode.ParameterCannotBeNull;
                }
            }
            else
            {
                Context[Key.User] = SharedUserLogic.RetrieveUser();
            }

            return result;
        }

        /// <summary>
        /// Populates the User and Card object within the current Context object.
        /// </summary>
        private void PopulateContextUserAndCard()
        {
            // Populate the Context InitialUser from the discovered User.
            User user = (User)Context[Key.User];
            Context[Key.InitialUser] = new User(user);

            RewardPrograms rewardPrograms = RewardPrograms.CardLinkOffers;
            if (Context.ContainsKey(Key.RewardProgramType) == true)
            {
                rewardPrograms = (RewardPrograms)Context[Key.RewardProgramType];
            }

            // Build a Card from the discovered User and specified NewCardInfo.
            NewCardInfo newCardInfo = (NewCardInfo)Context[Key.NewCardInfo];
            int year = newCardInfo.Expiration.Year;
            int month = newCardInfo.Expiration.Month;
            Context[Key.Card] = new Card(user.GlobalId)
            {
                NameOnCard = newCardInfo.NameOnCard,
                LastFourDigits = newCardInfo.Number.Substring(newCardInfo.Number.Length - 4, 4),
                Expiration = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                CardBrand = ParseCardBrand(newCardInfo.CardBrand),
                RewardPrograms = rewardPrograms
            };
        }

        /// <summary>
        /// Parses the card brand string into a CardBrand enum value.
        /// </summary>
        /// <param name="cardBrand">
        /// The card brand string to parse.
        /// </param>
        /// <returns>
        /// * A valid CardBrand enum value if successful.
        /// * Else returns CardBrand.Unknown.
        /// </returns>
        private static CardBrand ParseCardBrand(string cardBrand)
        {
            CardBrand result;

            if (Enum.TryParse<CardBrand>(cardBrand, true, out result) == false)
            {
                result = CardBrand.Unknown;
            }

            return result;
        }

        /// <summary>
        /// The modified values for credit card number digits to be used within Luhn's algorithm where appropriate.
        /// </summary>
        private static readonly int[] LuhnModifiedValues = { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };
 
        /// <summary>
        /// List of masks for rejecting cards populated from CommerceService Config value CardProviderRejectionMask
        /// </summary>
        private static string[] ListCardProviderRejectionMask = null;
    }
}