/*
Copyright (c) Microsoft Corporation. All rights reserved. 
Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
*/
// https://github.com/Azure/azure-documentdb-dotnet/blob/master/samples/code-samples/ServerSideScripts/JS/BulkImport.js

function addMerchants(merchants) {
    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();

    if (!merchants)
        throw new Error("Merchants collection passed in is null");

    var merchantsCount = merchants.length;
    if (merchantsCount == 0) {
        getContext().getResponseHeader().setBody("Merchants collection is empty");
    }

    var currentCount = 0;

    //go through the collection and create each merchant
    addMerchant(merchants[currentCount], callback);

    function addMerchant(merchant, callback) {

        //disable automatic id generaion. Merchant Id is set in the client code.
        var options = { disableAutomaticIdGeneration: true };

        var isAccepted = collection.createDocument(collectionLink, merchant, options, callback);

        // If the request was accepted, callback will be called. 
        // Otherwise report current count back to the client,  
        // which will call the script again with remaining set of docs. 
        // This condition will happen when this stored procedure has been running too long 
        // and is about to get cancelled by the server. This will allow the calling client 
        // to resume this batch from the point we got to before isAccepted was set to false 
        if (!isAccepted)
            getContext().getResponse().setBody(currentCount);

    }

    // This is called when collection.createDocument is done and the document has been persisted. 
    function callback(err, merchant, options) {
        if (err)
            throw err;

        // Merchant added successfully...increment the count. 
        currentCount++;

        if (currentCount >= merchantsCount) {
            // All merchants in the collection added to db 
            getContext().getResponse().setBody(currentCount);
        } else {
            // Merchants are still left in the collection. 
            addMerchant(merchants[currentCount], callback);
        }
    }

}