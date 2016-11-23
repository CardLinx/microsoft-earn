/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
(function () {
    "use strict";

    function valueOrDefault(d) {
        return d !== undefined ? d : 0;
    }

    function renderCheckbox(data, type) {
        if (type === 'display') {
            var checkedAttr = data ? "checked" : "";
            return '<input type="checkbox" disabled ' + checkedAttr + '>';
        }
        return data;
    }

    window.datatableDefinitions = {
        // transactions
        transactionsHistoryColumns: [
            { "title": "Transaction Link Id" },
            {
                "title": "Date Added (PST)",
                "render": function (data) {
                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            { "title": "Merchant Name" },
            {
                "title": "Transaction Type Id",
                "visible": false
            },
            { "title": "Transaction Type" },
            {
                "title": "Percent",
                "render": function (data) {
                    return data + "%";
                }
            },
            {
                "title": "Transaction<br> Amount",
                "render": function (data) {
                    return "$" + data.toFixed(2);
                }
            },

            {
                "title": "Discount<br> Amount",
                "render": function (data) {
                    return "$" + data.toFixed(2);
                }
            },
            { "title": "Last 4 Digits" },
            { "title": "Card" },
            {
                "title": "User Global Id",
                "render": function (data) {
                    return "<a href='/support/customer?customerGlobalId=" + data + "'>" + data + "</a>";
                }
            }
        ],

        transactionsHistoryRowCreateCallback: function (rawData) {
            return [
                valueOrDefault(rawData.transaction_id),
                valueOrDefault(rawData.date_added),
                valueOrDefault(rawData.merchant_name),
                valueOrDefault(rawData.transaction_type_id),
                valueOrDefault(rawData.transaction_type),
                valueOrDefault(rawData.percent),
                valueOrDefault(rawData.transaction_amount),
                valueOrDefault(rawData.discount_amount),
                valueOrDefault(rawData.last_4_digits),
                valueOrDefault(rawData.card_brand_name),
                valueOrDefault(rawData.user_global_id)
            ];
        },

        transactionsHistoryRowCreatedCallback: function (row, data, index) {
            var className = "";
            if (data[4] == "Earn") {
                className = "earn-transaction-type";
            } else {
                className = "burn-transaction-type";
            }
            $('td', row).eq(3).addClass(className);
            $('td', row).eq(4).addClass(className);
            $('td', row).eq(5).addClass(className);
            $('td', row).eq(6).addClass(className);
        },

        // settlements
        settlementsColumns: [
            { "title": "Partner Name" },
            { "title": "Card Brand" },
            { "title": "Last 4 Digits" },
            { "title": "Partner Merchant Id" },
            { "title": "Merchant Name" },
            {
                "title": "Authorization Date (Local)",
                "render": function (data) {
                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            {
                "title": "Authorization Date (UTC)",
                "render": function (data) {
                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            {
                "title": "Reached Terminal State (UTC)",
                "render": function (data) {
                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            {
                "title": "Transaction Type Id",
                "visible": false
            },
            { "title": "Transaction Type" },
            {
                "title": "Authorization<br> Amount",
                "render": function (data) {
                    //data = data == 0 ? 0.00 : data.toFixed(2)
                    return "$" + data.toFixed(2);
                }
            },
            {
                "title": "Settlement<br> Amount",
                "render": function (data) {
                    //data = data == 0 ? 0.00 : data.toFixed(2)
                    return "$" + data.toFixed(2);
                }
            },
            {
                "title": "Discount<br> Amount",
                "render": function (data) {
                    //data = data == 0 ? 0.00 : data.toFixed(2)
                    return "$" + data.toFixed(2);
                }
            },
            { "title": "Credit Status" },
            { "title": "CurrentState" },
            {
                "title": "User Global Id",
                "render": function (data) {
                    return "<a href='/support/customer?customerGlobalId=" + data + "'>" + data + "</a>";
                }
            }
        ],

        settlementsRowCreateCallback: function (rawData) {
            return [
                valueOrDefault(rawData.partner_name),
                valueOrDefault(rawData.card_brand_name),
                valueOrDefault(rawData.last_4_digits),
                valueOrDefault(rawData.partner_merchant_id),
                valueOrDefault(rawData.merchant_name),
                valueOrDefault(rawData.authorization_date_time_local),
                valueOrDefault(rawData.authorization_date_time_utc),
                valueOrDefault(rawData.utc_reached_terminal_state),
                valueOrDefault(rawData.transaction_type_id),
                valueOrDefault(rawData.transaction_type),
                valueOrDefault(rawData.authorization_amount),
                valueOrDefault(rawData.settlement_amount),
                valueOrDefault(rawData.discount_amount),
                valueOrDefault(rawData.credit_status),
                valueOrDefault(rawData.current_state),
                valueOrDefault(rawData.global_user_id)
            ]
        },

        settlementsRowCreatedCallback: function (row, data, index) {
            var transactionClassName = "";
            if (data[9] == "MicrosoftEarn") {
                transactionClassName = "earn-transaction-type";
            } else {
                transactionClassName = "burn-transaction-type";
            }

            var settlementStateClass = "";
            if (data[14] == "Succeeded") {
                settlementStateClass = "settlemetn-state-succedded";
            } else {
                settlementStateClass = "settlemetn-state-failed";
            }

            $('td', row).eq(8).addClass(transactionClassName);
            $('td', row).eq(9).addClass(transactionClassName);
            $('td', row).eq(10).addClass(transactionClassName);
            $('td', row).eq(11).addClass(transactionClassName);
            $('td', row).eq(13).addClass(settlementStateClass);
        },

        // customers
        foundCustomersColumns: [
            { "title": "User Id (Hex)" },
            { "title": "Global User Id" },
            { "title": "Email" },
            { "title": "MSID" },
            { "title": "Phone Number" }
        ],

        foundCustomersRowCreateCallback: function (rawData) {
            return [
                valueOrDefault(rawData.id_hex),
                valueOrDefault(rawData.global_id),
                valueOrDefault(rawData.email),
                valueOrDefault(rawData.msid),
                valueOrDefault(rawData.phone_number)
            ];
        },

        // referrals
        referralsColumns: [
            { "title": "Agent Id" },
            { "title": "Reward Reason" },
            { "title": "Reward Payout Status" },
            { "title": "Explanation" },
            {
                "title": "Payout Scheduled Date (UTC)",
                "render": function (data) {
                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            {
                "title": "Payout Finalized Date (UTC)",
                "render": function (data) {
                    if (data === null)
                        return "";

                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            {
                "title": "Amount",
                "render": function (data) {
                    return "$" + data.toFixed(2);
                }
            },
        ],

        referralsRowCreateCallback: function (rawData) {
            return [
                valueOrDefault(rawData.agent_id),
                valueOrDefault(rawData.reward_reason),
                valueOrDefault(rawData.reward_payout_status),
                valueOrDefault(rawData.explanation),
                valueOrDefault(rawData.payout_scheduled_date_utc),
                valueOrDefault(rawData.payout_finalized_date_utc),
                valueOrDefault(rawData.amount)
            ];
        },

        // earn burn line items
        earnBurnLineItemsColumns: [
            { "title": "Transaction Id" },
            {
                "title": "Transaction Date",
                "render": function (data) {
                    if (data === null)
                        return "";

                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            {
                "title": "Earn Credit",
                "render": function (data) {
                    return "$" + data.toFixed(2);
                }
            },
            {
                "title": "Burn Debit",
                "render": function (data) {
                    return "$" + data.toFixed(2);
                }
            },
            {
                "title": "Has Redeemed<br> Deal Record",
                "render": function (data, type, row) {
                    return renderCheckbox(data, type);
                },
                className: "dt-body-center"
            },
            { "title": "Transaction Type" },
            {
                "title": "Deal Percent",
                "render": function (data) {
                    return data.toFixed(2) + "%";
                }
            },
            { "title": "Merchant Name" },
            {
                "title": "Transaction Amount",
                "render": function (data) {
                    return "$" + data.toFixed(2);
                }
            },
            {
                "title": "Reversed",
                "render": function (data, type, row) {
                    return renderCheckbox(data, type);
                },
                className: "dt-body-center"
            },
            { "title": "Transaction<br>Status" },
            { "title": "Last 4<br>Digits" },
            { "title": "Card<br>Brand" },
            {
                "title": "Perma<br>Pending",
                "render": function (data, type, row) {
                    return renderCheckbox(data, type);
                },
                className: "dt-body-center"
            },
            { "title": "Review<br>Status" },
            { "title": "redeem<br>Deal Id" }
        ],

        earnBurnLineItemsRowCreateCallback: function (rawData) {
            return [
                valueOrDefault(rawData.transaction_id),
                valueOrDefault(rawData.transaction_date),
                valueOrDefault(rawData.earn_credit),
                valueOrDefault(rawData.burn_debit),
                valueOrDefault(rawData.has_redeemed_deal_record),
                valueOrDefault(rawData.transaction_type),
                valueOrDefault(rawData.deal_percent),
                valueOrDefault(rawData.merchant_name),
                valueOrDefault(rawData.transaction_amount),
                valueOrDefault(rawData.reversed),
                valueOrDefault(rawData.transaction_status),
                valueOrDefault(rawData.last_4_digits),
                valueOrDefault(rawData.card_brand),
                valueOrDefault(rawData.perma_pending),
                valueOrDefault(rawData.review_status),
                valueOrDefault(rawData.redeem_deal_id)
            ];
        },

        // earn burn history
        earnBurnHistoryColumns: [
            { "title": "Reimbursement Tender" },
            {
                "title": "Purchase Date",
                "render": function (data) {
                    if (data === null)
                        return "";

                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            { "title": "Merchant Name" },
            {
                "title": "Percent",
                "render": function (data) {
                    return data.toFixed(2) + "%";
                }
            },
            {
                "title": "Authorization Amount",
                "render": function (data) {
                    return "$" + data.toFixed(2);
                }
            },
            {
                "title": "Discount Amount",
                "render": function (data) {
                    return "$" + data.toFixed(2);
                }
            },
            { "title": "Last 4 Digits" },
            { "title": "Card Brand" },
            {
                "title": "Reversed",
                "render": function (data, type, row) {
                    return renderCheckbox(data, type);
                },
            },
            { "title": "Credit Status" }
        ],

        earnBurnHistoryRowCreateCallback: function (rawData) {
            return [
                valueOrDefault(rawData.reimbursement_tender),
                valueOrDefault(rawData.purchase_datetime),
                valueOrDefault(rawData.merchant_name),
                valueOrDefault(rawData.percent),
                valueOrDefault(rawData.authorization_amount),
                valueOrDefault(rawData.discount_amount),
                valueOrDefault(rawData.last_four_digits),
                valueOrDefault(rawData.card_brand),
                valueOrDefault(rawData.reversed),
                valueOrDefault(rawData.credit_status)
            ];
        },

        // merchants
        businessesColumns: [
            {
                "title": "Id",
                "visible": false,
                "searchable": false
            },
            {
                "title": "Active",
                "visible": false,
                "searchable": false
            },
            { "title": "Name" },
            { "title": "Location" },
            {
                "title": "Payment",
                "render": function (data, type, row) {

                    var result = "";
                    if (type === 'export') {
                        if (data.visa.valid) {
                            result += 'Visa';
                        }
                        if (data.mc.valid) {
                            if (result.length > 0) {
                                result += ', ';
                            }
                            result += 'Master Card';
                        }
                        if (data.amex.valid) {
                            if (result.length > 0) {
                                result += ', ';
                            }
                            result += ' American Express';
                        }
                    } else {
                        if (data.visa.valid) {
                            result += '<img src="../../../Content/img/visa.png" style="padding-right: 10px;" />';
                        }
                        if (data.mc.valid) {
                            result += '<img src="../../../Content/img/mc.png" style="padding-right: 10px;"/>';
                        }
                        if (data.amex.valid) {
                            result += '<img src="../../../Content/img/amex.png" style="padding-right: 10px;"/>';
                        }
                    }

                    return result;
                },
            },
            {
                "title": "Payment_search",
                "visible": false,
                "searchable": true,
                "render": function (data) {

                    var result = "";
                    if (data.visa.valid) {
                        result += 'visa';
                    }
                    if (data.mc.valid) {
                        result += ' mc master card mastercard';
                    }
                    if (data.amex.valid) {
                        result += ' amex americanexpress american express';
                    }
                    return result;
                },
            },
            {
                "title": "Top Merchant",
                "width": "100",
                "render": function (data, type, row) {
                    return renderCheckbox(data, type);
                }
            },
        ],

        businessesRowCreateCallback: function (rawData) {
            var payments = {
                visa: {
                    valid: rawData.validVisa(),
                    //syncedWithCommerce: rawData.visaIsSyncedWithCommerce()
                },
                mc: {
                    valid: rawData.validMasterCard(),
                    //syncedWithCommerce: rawData.mcIsSyncedWithCommerce()
                },
                amex: {
                    valid: rawData.validAmex(),
                    //syncedWithCommerce: rawData.amexIsSyncedWithCommerce()
                }
            }

            return [
                valueOrDefault(rawData.id()),
                valueOrDefault(rawData.active()),
                valueOrDefault(rawData.name()),
                valueOrDefault(rawData.locationText()),
                valueOrDefault(payments),
                valueOrDefault(payments),
                valueOrDefault(rawData.rank()),
            ];
        },

        businessesRowCreatedCallback: function (row, data, index) {
            var className = "merchant-inactive";
            if (data[1]) {
                className = "merchant-active";
            }

            $('td', row).parent().addClass(className);

            $(row).attr('data-toggle', 'modal');
            $(row).attr('data-target', '#merchantModal');
            $(row).css('cursor', 'pointer');
            $(row).attr('data-id', data[0]);

            $('td', row).eq(3).addClass("checkbox-center");
        },

        webAnalyticsColumns: [
            { "title": "IP Address" },
            { "title": "Session Id" },
            { "title": "Device Type" },
            { "title": "Page Title" },
            { "title": "Event Id" },
            {
                "title": "Server Timestamp",
                "render": function (data) {
                    if (data === null)
                        return "";

                    var date = new Date(data);
                    return date.fullDateToString();
                }
            },
            { "title": "cmp_ref" },
            {
                "title": "New User?",
                "render": function (data, type, row) {
                    return renderCheckbox(data, type);
                },
            },
            {
                "title": "Is Authenticated?",
                "render": function (data, type, row) {
                    return renderCheckbox(data, type);
                },
            },
        ],

        webAnalyticsRowCreateCallback: function (rawData) {
            return [
                valueOrDefault(rawData.IPAddress),
                valueOrDefault(rawData.SessionId),
                valueOrDefault(rawData.DeviceType),
                valueOrDefault(rawData.PageTitle),
                valueOrDefault(rawData.EventId),
                valueOrDefault(rawData.ServerTimeStamp),
                valueOrDefault(rawData.cmp_ref),
                valueOrDefault(rawData.NewUser),
                valueOrDefault(rawData.IsAuthenticated)
            ];
        }
    };

}());