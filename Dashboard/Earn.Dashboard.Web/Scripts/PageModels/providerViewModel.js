/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {
    window.providerViewModel = function (providerId, providerName, providerOfferId, isNational) {
        self.merchantsTable = new window.extendedTableWidget(
                '#businessesTable',
                window.datatableDefinitions.businessesColumns,
                1, 1, 'desc', 50, false,
                undefined, undefined, undefined, window.datatableDefinitions.businessesRowCreateCallback, window.datatableDefinitions.businessesRowCreatedCallback);

        self.providerId = ko.observable(providerId);
        self.providerName = ko.observable(providerName),
        self.providerOfferId = ko.observable(providerOfferId);

        self.isNational = ko.observable(isNational);

        self.offers = ko.observableArray([]);
        self.merchants = ko.observableArray([]);
        self.selectedMerchant = ko.observable();
        self.selectedOffer = ko.observable();
        self.merchantsFilter = ko.observable("");

        self.updateProviderStatus = function (status) {
            $('#spinner').show();
            var dfd = jQuery.Deferred();
            $.ajax({
                type: "POST",
                url: '/providers/updateproviderstatus/' + self.providerId(),
                contentType: "application/json",
                data: JSON.stringify({ status: status })
            }).done(function (response) {
                if (status) {
                    toastr.success('Activated provider successfully', 'Activate Provider', { timeOut: 2000 });
                } else {
                    toastr.warning('Inactivated provider successfully', 'Activate Provider', { timeOut: 2000 });
                }
                dfd.resolve();
            }).fail(function (error) {
                toastr.error('Failed to activate provider', 'Inactivate Provider', { timeOut: 2000 });
                dfd.reject();
            }).always(function () {
                $('#spinner').hide();
            });

            return dfd.promise();
        }

        self.updateOfferInfo = function () {
            $('#spinner').show();
            var dfd = jQuery.Deferred();
            $.ajax({
                type: "GET",
                url: '/providers/getproviderbyid/' + self.providerId(),
                contentType: "application/json",
            }).done(function (response) {
                var result = JSON.parse(response);
                self.providerOfferId(result.offer_id);
                dfd.resolve();
            }).fail(function (error) {
                toastr.error('Failed to load offers', 'Load Offers', { timeOut: 2000 });
                dfd.reject();
            }).always(function () {
                $('#spinner').hide();
            });

            return dfd.promise();
        }

        self.loadOffers = function () {
            $('#spinner').show();
            self.offers([]);
            $.ajax({
                type: "GET",
                url: '/providers/getoffers/' + self.providerId(),
                contentType: "application/json",
            }).done(function (response) {
                var result = JSON.parse(response);
                if (result && $.isArray(result)) {
                    $.each(result, function (i, offer) {
                        offer.is_active = offer.id == self.providerOfferId();
                        self.offers.push(new window.offerViewModel(offer));
                    });
                }
            }).fail(function (error) {
                toastr.error('Failed to load offers', 'Load Offers', { timeOut: 2000 });
            }).always(function () {
                $('#spinner').hide();
            });
        }

        self.loadMerchants = function () {
            $('#spinner').show();
            self.merchants([]);
            $.ajax({
                type: "GET",
                url: '/providers/getmerchants?id=' + self.providerId() + "&query=" + self.merchantsFilter(),
                contentType: "application/json",
            }).done(function (response) {
                var result = JSON.parse(response);
                if (result && $.isArray(result)) {
                    $.each(result, function (i, merchant) {
                        self.merchants.push(new window.merchantViewModel(merchant));
                    });
                }
                self.merchantsTable.updateData(self.merchants());
            }).fail(function (error) {
                toastr.error('Failed to load merchants', 'Load Merchants', { timeOut: 2000 });
            }).always(function () {
                $('#spinner').hide();
            });
        }

        self.initUI = function () {
            if (self.isNational()) {
                self.merchantsTable.table.column(6).visible(false);
            }

            self.loadOffers();
            self.loadMerchants();

            $('#saveOfferForm').validate({
                debug: false,
                errorClass: 'text-danger',
                errorElement: 'span',
                rules: {
                    title: {
                        noSpace: true,
                        required: true
                    },
                    discount: {
                        noSpace: true,
                        required: true,
                        range: [0, 100]
                    },
                },
                messages: {
                    title: 'Please enter the Offer Title',
                    discount: 'Please enter a valid Offer Discount'
                }
            });

            $('#saveMerchantForm').validate({
                debug: false,
                errorClass: 'text-danger',
                errorElement: 'span',
                rules: {
                    name: {
                        noSpace: true,
                        required: true
                    },
                    visa: {
                        visaMids: true
                    },
                    mc: {
                        mcMids: true
                    },
                    mcLocations: {
                        mcLocations: true
                    },
                    amex: {
                        amexMids: true
                    }
                },
                messages: {
                    name: 'Please enter the Merchant Name'
                }
            });

            // -- Query Builder --
            // hide not necessaty operator for visa, amex, master card
            $(document).on('change', 'select[name$="filter"]', function () {
                var val = $(this).val();
                if ((val == "Visa.Is") || 
                    (val == "MasterCard.Is") ||
                    (val == "Amex.Is")) {
                    $(this).parent().next().hide();
                } else {
                    $(this).parent().next().show();
                }
            });

            var paymentInput = function (rule, name) {
                var $container = rule.$el.find('.rule-value-container');
                return '\
                                <label>is</label> \
                                <select class="form-control" name="' + name + '_1"> \
                                    <option value="Valid">Valid</option> \
                                    <option value="Invalid">Invalid</option> \
                                    <option value="Synced">Synced</option> \
                                    <option value="PartiallySynced">Partially Synced</option> \
                                    <option value="NotSynced">Not Synced</option> \
                                </select>';
            };

            var paymentValueGetter = function (rule) {
                return rule.$el.find('.rule-value-container [name$=_1]').val();
            };

            var paymentValueSetter = function (rule, value) {
                if (rule.operator.nb_inputs > 0) {
                    var val = value.split('.');
                    rule.$el.find('.rule-value-container [name$=_1]').val(val[0]).trigger('change');
                }
            };

            $('#queryBuilder').queryBuilder({
                optgroups: {
                    merchant: '-- Merchant',
                    payment: '-- Payment'
                },
                filters: [
                {
                    optgroup: 'payment',
                    id: 'Visa.Is',
                    label: 'Visa',
                    type: 'string',
                    operators: ['equal'],
                    input: paymentInput,
                    valueGetter: paymentValueGetter,
                    valueSetter: paymentValueSetter
                },
                {
                    optgroup: 'payment',
                    id: 'MasterCard.Is',
                    label: 'Master Card',
                    type: 'string',
                    operators: ['equal'],
                    input: paymentInput,
                    valueGetter: paymentValueGetter,
                    valueSetter: paymentValueSetter
                },
                {
                    optgroup: 'payment',
                    id: 'Amex.Is',
                    label: 'Amex',
                    type: 'string',
                    operators: ['equal'],
                    input: paymentInput,
                    valueGetter: paymentValueGetter,
                    valueSetter: paymentValueSetter
                },
                {
                    id: 'Active',
                    optgroup: 'merchant',
                    label: 'Active',
                    type: 'boolean',
                    input: 'radio',
                    values: {
                        true: 'True',
                        false: 'False'
                    },
                    operators: ['equal']
                },
                {
                    id: 'ValidLocation',
                    optgroup: 'merchant',
                    label: 'Location',
                    type: 'boolean',
                    input: 'radio',
                    values: {
                        true: 'Valid',
                        false: 'Invalid'
                    },
                    operators: ['equal']
                },
                {
                    id: 'Name',
                    optgroup: 'merchant',
                    label: 'Name',
                    type: 'string',
                    operators: ['equal']
                },
                {
                    id: 'Id',
                    optgroup: 'merchant',
                    label: 'Identifier',
                    type: 'string',
                    operators: ['equal'],
                    validation: {
                        format: /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/
                    }
                }]
            });
        };

        $('#merchantModal').on('shown.bs.modal', function (e) {
            var index = $(e.relatedTarget).data('id')
            if (index == -1) {
                $('#merchantModalLabel').text("New Merchant");
                self.selectedMerchant(new window.merchantViewModel({ is_active: true }));
            } else {
                $('#merchantModalLabel').text("Edit Merchant");

                var merchant;
                for (var i = 0; i < self.merchants().length; i++) {
                    if (self.merchants()[i].id() == index) {
                        self.selectedMerchant(self.merchants()[i].clone());
                        break;
                    }
                }
            }
        });

        $('#offerModal').on('shown.bs.modal', function (e) {
            var index = $(e.relatedTarget).data('id')
            if (index == -1) {
                $('#offerModalLabel').text("New Offer");
                self.selectedOffer(new window.offerViewModel({ is_active: self.offers().length == 0 }));
            } else {
                $('#offerModalLabel').text("Edit Offer");
                self.selectedOffer(self.offers()[index].clone());
            }
        });

        $('#enablePovider').click(function (event) {
            if (this.checked) {
                if (self.providerOfferId() && self.offers().length > 0) {
                    if (window.confirm("Are you sure, you want to Activate this provider?")) {
                        self.updateProviderStatus(true);
                        return true;
                    }
                } else {
                    toastr.error('Please add an offer to proceed', 'Activate Provider', { timeOut: 3000 });
                }
            } else {
                if (window.confirm("Are you sure, you want to Inactivate this provider?")) {
                    self.updateProviderStatus(false);
                    return true;
                }
            }

            event.preventDefault();
            event.stopPropagation();
            return false;
        });

        $('#exportMerchants').click(function () {
            var url = '/api/providers/exportmerchants/?id=' + self.providerId() + '&paymentProcessor=' + $("input:radio[name=export]:checked").val() + '&query=' + self.merchantsFilter() + '&providerName=' + self.providerName();
            $('#exportMerchants').addClass('disabled');
            $('#spinner').show();
            window.location = url;
            $('#spinner').hide();
            $('#exportMerchants').removeClass('disabled');
        });

        self.addOrUpdateMerchant = function (e, e2) {
            var validator = $('#saveMerchantForm').validate();
            if (!validator.valid()) {
                return;
            }

            var data = {
                ProviderId: self.providerId(),
                Id: self.selectedMerchant().id(),
                Name: self.selectedMerchant().name(),
                Url: self.selectedMerchant().url(),
                Location: {
                    address: self.selectedMerchant().address(),
                    city: self.selectedMerchant().city(),
                    state: self.selectedMerchant().state(),
                    zip: self.selectedMerchant().zip(),
                    latitude: self.selectedMerchant().latitude(),
                    longitude: self.selectedMerchant().longitude()
                },
                PhoneNumber: self.selectedMerchant().phone(),
                Payments: self.selectedMerchant().payments(),
                IsActive: self.selectedMerchant().active(),
                Rank: self.selectedMerchant().rank(),
            };

            var jsonData = JSON.stringify(data);

            $('#spinner').show();
            $.ajax({
                type: "POST",
                url: '/providers/addmerchant/',
                contentType: "application/json",
                data: jsonData,
            }).done(function (response) {
                toastr.success('Saved merchant successfully', 'Save Merchant', { timeOut: 2000 });
                self.loadMerchants();
            }).fail(function (error) {
                toastr.error('Failed to save merchant', 'Save Merchant', { timeOut: 2000 });
            }).always(function () {
                $('#spinner').hide();
                $('#merchantModal').modal('hide');
            });

        }

        self.addOrUpdateOffer = function (e, e2) {
            var validator = $('#saveOfferForm').validate();
            if (!validator.valid()) {
                return;
            }

            var data = {
                Id: self.selectedOffer().id(),
                Title: self.selectedOffer().title(),
                Discount: self.selectedOffer().discount()
            };

            data.ProviderId = self.providerId();
            data.ProviderName = self.providerName();

            $('#spinner').show();
            $.ajax({
                type: "POST",
                url: '/providers/addoffer/?active=' + self.selectedOffer().active(),
                contentType: "application/json",
                data: JSON.stringify(data)
            }).done(function (response) {
                toastr.success('Saved OFFER successfully', 'Save Offer', { timeOut: 2000 });
                self.updateOfferInfo().done(function () {
                    self.loadOffers();
                });
            }).fail(function (error) {
                toastr.error('Failed to save offer', 'Save Offer', { timeOut: 2000 });
            }).always(function () {
                $('#spinner').hide();
                $('#offerModal').modal('hide');
            });
        },

        self.queryBuilderFilter = function () {
            query = $('#queryBuilder').queryBuilder('getSQL', false);
            if (query.sql.length) {
                self.merchantsFilter(query.sql);
                self.loadMerchants();
            }
        };

        self.queryBuilderSql = function () {
            var result = $('#queryBuilder').queryBuilder('getSQL', false);
            alert(result.sql);
        };

        self.queryBuilderReset = function () {
            $('#queryBuilder').queryBuilder('reset');
            self.merchantsFilter('');
            self.loadMerchants();
        };

        self.initUI();
    }
}());