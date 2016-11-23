//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // 
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OffersEmail.FrontEnd.Controllers.ModelBinders
{
    using System.IO;
    using System.Web.Mvc;

    using Newtonsoft.Json;

    /// <summary>  
    /// Model binder that does the mapping for any JSON request or basic request   
    /// </summary> 
    public class JsonModelBinder : IModelBinder
    {
        /// <summary>
        /// Bind the model action
        /// </summary>
        /// <param name="controllerContext">the controller context</param>
        /// <param name="bindingContext">the binding context</param>
        /// <returns>the deserialized object</returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json"))
            {
                return new DefaultModelBinder().BindModel(controllerContext, bindingContext);
            }

            var request = controllerContext.HttpContext.Request;
            var jsonStringData = new StreamReader(request.InputStream).ReadToEnd();

            return JsonConvert.DeserializeObject(jsonStringData, bindingContext.ModelMetadata.ModelType);
        }
    }
}