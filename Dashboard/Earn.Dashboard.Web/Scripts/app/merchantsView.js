/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
$(function () {
    var providerId = $('#providerId').val(),
        providerName = $('#providerName').val(),
        providerOfferId = $('#providerOfferId').val(),
        isNational = $('#isNational')[0].checked;

    $('#confirmImportModal').on('hidden.bs.modal', function () {
        $('#importMerchantsUpload').val('');
    });

    var fileToImport;
    $('#confirmImport').click(function () {
        $('#confirmImportModal').modal('hide');
        if (fileToImport) {
            var data = new FormData();
            data.append("id", providerId);
            data.append("merchantFileType", $("input:radio[name=import]:checked").val());
            data.append("file", fileToImport);
            $('#importMerchants').addClass('disabled');
            $('#importMerchantsUpload').prop('disabled', true);
            $('#spinner').show();
            $.ajax({
                type: "POST",
                url: '/providers/addmerchants/',
                contentType: false,
                processData: false,
                cache: false,
                data: data
            }).done(function (response) {
                toastr.success('Scheduled import job successfully. \n"' + response + '" ', 'Import Merchants', { timeOut: 2000 });
            }).fail(function (error) {
                toastr.error('Failed to load file', 'Import Merchants', { timeOut: 2000 });
            }).always(function () {
                $('#spinner').hide();
                $('#importMerchantsUpload').prop('disabled', false);
                $('#importMerchants').removeClass('disabled');
            });
        }
    });

    $('#importMerchantsUpload').change(function (e) {
        var files = e.target.files;
        if (files.length > 0) {
            fileToImport = files[0];
            fileExt = fileToImport.name.substring(fileToImport.name.lastIndexOf('.'));
            if (fileExt && (fileExt.toLowerCase() == '.xlsx') || (fileExt.toLowerCase() == '.csv')) {
                $('#confirmImportFileName').text(fileToImport.name);
                $('#confirmImportFileFormat').text($("input:radio[name=import]:checked").val());
                $('#confirmImportModal').modal('show');
            } else {
                toastr.error('Please select .xlsx or .csv file', 'Import Merchants', { timeOut: 2000 });
            }
        }
    });    

    ko.applyBindings(new window.providerViewModel(providerId, providerName, providerOfferId, isNational), document.getElementById("mainSection"));
})