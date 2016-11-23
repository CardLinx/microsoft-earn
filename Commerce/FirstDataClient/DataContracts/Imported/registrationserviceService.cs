//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
//This file is NOT auto-generated. First Data WSDLs have been notoriously difficult to work with. SVCUTIL does not import them
//properly, and no one has been able to determine why. Rather than rathole on auto-generating these classes, this file will be
//maintained by hand. GeneratedCode attribute is included below to keep CodeAnalysis from complaining-- First Data controls these
//objects, so there's no cause to chase CodeAnalysis issues.

using System.Net.Security;
[System.ServiceModel.ServiceContractAttribute(Namespace="http://api.offers.firstdata.com/v1/card", ConfigurationName="registrationservice")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public interface registrationservice
{

    [System.ServiceModel.OperationContractAttribute(Action = "", ReplyAction = "*", ProtectionLevel = ProtectionLevel.None)]
    [System.ServiceModel.XmlSerializerFormatAttribute()]
    OfferRegisterResponse1 OfferRegister(OfferRegisterRequest1 request);

    [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
    System.Threading.Tasks.Task<OfferRegisterResponse1> OfferRegisterAsync(OfferRegisterRequest1 request);

    [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*", ProtectionLevel=ProtectionLevel.None)]
    [System.ServiceModel.XmlSerializerFormatAttribute()]
    CardRegisterResponse1 CardRegister(CardRegisterRequest1 request);

    [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
    System.Threading.Tasks.Task<CardRegisterResponse1> CardRegisterAsync(CardRegisterRequest1 request);
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=false)]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class OfferRegisterRequest
{

    private OfferRegisterRequestReqType reqTypeField;

    private string provIDField;

    private string pubIDField;

    private string pubNameField;

    private string offerIDField;

    private string offerNameField;

    private string coalitionIDField;

    private OfferRegisterRequestOffer offerField;

    private OfferRegisterRequestConsumer consumerField;

    /// <remarks/>
    public OfferRegisterRequestReqType reqType
    {
        get
        {
            return this.reqTypeField;
        }
        set
        {
            this.reqTypeField = value;
        }
    }

    /// <remarks/>
    public string provID
    {
        get
        {
            return this.provIDField;
        }
        set
        {
            this.provIDField = value;
        }
    }

    /// <remarks/>
    public string pubID
    {
        get
        {
            return this.pubIDField;
        }
        set
        {
            this.pubIDField = value;
        }
    }

    /// <remarks/>
    public string pubName
    {
        get
        {
            return this.pubNameField;
        }
        set
        {
            this.pubNameField = value;
        }
    }

    /// <remarks/>
    public string offerID
    {
        get
        {
            return this.offerIDField;
        }
        set
        {
            this.offerIDField = value;
        }
    }

    /// <remarks/>
    public string offerName
    {
        get
        {
            return this.offerNameField;
        }
        set
        {
            this.offerNameField = value;
        }
    }

    /// <remarks/>
    public string coalitionID
    {
        get
        {
            return this.coalitionIDField;
        }
        set
        {
            this.coalitionIDField = value;
        }
    }

    /// <remarks/>
    public OfferRegisterRequestOffer offer
    {
        get
        {
            return this.offerField;
        }
        set
        {
            this.offerField = value;
        }
    }

    /// <remarks/>
    public OfferRegisterRequestConsumer consumer
    {
        get
        {
            return this.consumerField;
        }
        set
        {
            this.consumerField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://api.offers.firstdata.com/v1/offer")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public enum OfferRegisterRequestReqType
{

    /// <remarks/>
    A,

    /// <remarks/>
    R,

    /// <remarks/>
    D,

    /// <remarks/>
    U,

    /// <remarks/>
    O,

    /// <remarks/>
    X,
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://api.offers.firstdata.com/v1/offer")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class OfferRegisterRequestOffer
{

    private string bECodeField;

    private string[] mIDGroupField;

    private string startDateTimeField;

    private string endDateTimeField;

    private string offerTypeField;

    private bool offerTypeFieldSpecified;

    private string minTxnAmtField;

    private string discountAmtField;

    private decimal discountPercentageField;

    private bool discountPercentageFieldSpecified;

    private int maxRedCountField;

    private bool maxRedCountFieldSpecified;

    private string offerModeField;

    /// <remarks/>
    public string BECode
    {
        get
        {
            return this.bECodeField;
        }
        set
        {
            this.bECodeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("MID", IsNullable=false)]
    public string[] MIDGroup
    {
        get
        {
            return this.mIDGroupField;
        }
        set
        {
            this.mIDGroupField = value;
        }
    }

    /// <remarks/>
    public string startDateTime
    {
        get
        {
            return this.startDateTimeField;
        }
        set
        {
            this.startDateTimeField = value;
        }
    }

    /// <remarks/>
    public string endDateTime
    {
        get
        {
            return this.endDateTimeField;
        }
        set
        {
            this.endDateTimeField = value;
        }
    }

    /// <remarks/>
    public string offerType
    {
        get
        {
            return this.offerTypeField;
        }
        set
        {
            this.offerTypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool offerTypeSpecified
    {
        get
        {
            return this.offerTypeFieldSpecified;
        }
        set
        {
            this.offerTypeFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
    public string minTxnAmt
    {
        get
        {
            return this.minTxnAmtField;
        }
        set
        {
            this.minTxnAmtField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string discountAmt
    {
        get
        {
            return this.discountAmtField;
        }
        set
        {
            this.discountAmtField = value;
        }
    }

    /// <remarks/>
    public decimal discountPercentage
    {
        get
        {
            return this.discountPercentageField;
        }
        set
        {
            this.discountPercentageField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool discountPercentageSpecified
    {
        get
        {
            return this.discountPercentageFieldSpecified;
        }
        set
        {
            this.discountPercentageFieldSpecified = value;
        }
    }

    /// <remarks/>
    public int maxRedCount
    {
        get
        {
            return this.maxRedCountField;
        }
        set
        {
            this.maxRedCountField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool maxRedCountSpecified
    {
        get
        {
            return this.maxRedCountFieldSpecified;
        }
        set
        {
            this.maxRedCountFieldSpecified = value;
        }
    }

    /// <remarks/>
    public string offerMode
    {
        get
        {
            return this.offerModeField;
        }
        set
        {
            this.offerModeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://api.offers.firstdata.com/v1/offer")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class OfferRegisterRequestConsumer
{

    private string tokenField;

    private string tokenTypeField;

    private string pANField;

    private string keyIDField;

    private string encryptPANField;

    private string cardSuffixField;

    private string consumerIDField;

    private string purPriceField;

    private string purDateTimeField;

    private string currCodeField;

    private OfferRegisterRequestConsumerOptOut optOutField;

    private bool optOutFieldSpecified;

    private string offerAcceptIDField;

    /// <remarks/>
    public string token
    {
        get
        {
            return this.tokenField;
        }
        set
        {
            this.tokenField = value;
        }
    }

    /// <remarks/>
    public string tokenType
    {
        get
        {
            return this.tokenTypeField;
        }
        set
        {
            this.tokenTypeField = value;
        }
    }

    /// <remarks/>
    public string PAN
    {
        get
        {
            return this.pANField;
        }
        set
        {
            this.pANField = value;
        }
    }

    /// <remarks/>
    public string keyID
    {
        get
        {
            return this.keyIDField;
        }
        set
        {
            this.keyIDField = value;
        }
    }

    /// <remarks/>
    public string encryptPAN
    {
        get
        {
            return this.encryptPANField;
        }
        set
        {
            this.encryptPANField = value;
        }
    }

    /// <remarks/>
    public string cardSuffix
    {
        get
        {
            return this.cardSuffixField;
        }
        set
        {
            this.cardSuffixField = value;
        }
    }

    /// <remarks/>
    public string consumerID
    {
        get
        {
            return this.consumerIDField;
        }
        set
        {
            this.consumerIDField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
    public string purPrice
    {
        get
        {
            return this.purPriceField;
        }
        set
        {
            this.purPriceField = value;
        }
    }

    /// <remarks/>
    public string purDateTime
    {
        get
        {
            return this.purDateTimeField;
        }
        set
        {
            this.purDateTimeField = value;
        }
    }

    /// <remarks/>
    public string currCode
    {
        get
        {
            return this.currCodeField;
        }
        set
        {
            this.currCodeField = value;
        }
    }

    /// <remarks/>
    public OfferRegisterRequestConsumerOptOut optOut
    {
        get
        {
            return this.optOutField;
        }
        set
        {
            this.optOutField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool optOutSpecified
    {
        get
        {
            return this.optOutFieldSpecified;
        }
        set
        {
            this.optOutFieldSpecified = value;
        }
    }

    /// <remarks/>
    public string offerAcceptID
    {
        get
        {
            return this.offerAcceptIDField;
        }
        set
        {
            this.offerAcceptIDField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://api.offers.firstdata.com/v1/offer")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public enum OfferRegisterRequestConsumerOptOut
{

    /// <remarks/>
    Y,

    /// <remarks/>
    N,
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://api.offers.firstdata.com/v1/offer")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class OfferRegisterResponse
{

    private string reqTypeField;

    private string tokenField;

    private string tokenTypeField;

    private string cardSuffixField;

    private string provIDField;

    private string offerIDField;

    private string consumerIDField;

    private string coalitionIDField;

    private string respCodeField;

    private string respTextField;

    /// <remarks/>
    public string reqType
    {
        get
        {
            return this.reqTypeField;
        }
        set
        {
            this.reqTypeField = value;
        }
    }

    /// <remarks/>
    public string token
    {
        get
        {
            return this.tokenField;
        }
        set
        {
            this.tokenField = value;
        }
    }

    /// <remarks/>
    public string tokenType
    {
        get
        {
            return this.tokenTypeField;
        }
        set
        {
            this.tokenTypeField = value;
        }
    }

    /// <remarks/>
    public string cardSuffix
    {
        get
        {
            return this.cardSuffixField;
        }
        set
        {
            this.cardSuffixField = value;
        }
    }

    /// <remarks/>
    public string provID
    {
        get
        {
            return this.provIDField;
        }
        set
        {
            this.provIDField = value;
        }
    }

    /// <remarks/>
    public string offerID
    {
        get
        {
            return this.offerIDField;
        }
        set
        {
            this.offerIDField = value;
        }
    }

    /// <remarks/>
    public string consumerID
    {
        get
        {
            return this.consumerIDField;
        }
        set
        {
            this.consumerIDField = value;
        }
    }

    /// <remarks/>
    public string coalitionID
    {
        get
        {
            return this.coalitionIDField;
        }
        set
        {
            this.coalitionIDField = value;
        }
    }

    /// <remarks/>
    public string respCode
    {
        get
        {
            return this.respCodeField;
        }
        set
        {
            this.respCodeField = value;
        }
    }

    /// <remarks/>
    public string respText
    {
        get
        {
            return this.respTextField;
        }
        set
        {
            this.respTextField = value;
        }
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class OfferRegisterRequest1
{

    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.offers.firstdata.com/v1/offer", Order=0)]
    public OfferRegisterRequest OfferRegisterRequest;

    public OfferRegisterRequest1()
    {
    }

    public OfferRegisterRequest1(OfferRegisterRequest OfferRegisterRequest)
    {
        this.OfferRegisterRequest = OfferRegisterRequest;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class OfferRegisterResponse1
{

    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.offers.firstdata.com/v1/offer", Order=0)]
    public OfferRegisterResponse OfferRegisterResponse;

    public OfferRegisterResponse1()
    {
    }

    public OfferRegisterResponse1(OfferRegisterResponse OfferRegisterResponse)
    {
        this.OfferRegisterResponse = OfferRegisterResponse;
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://api.offers.firstdata.com/v1/card")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class CardRegisterRequest
{

    private string provIDField;

    private CardRegisterRequestReqType reqTypeField;

    private string pANField;

    private string keyIDField;

    private string encryptPANField;

    private string consumerIDField;

    private string pubIDField;

    private string pubNameField;

    private CardRegisterRequestCoalitionList coalitionListField;

    /// <remarks/>
    public string provID
    {
        get
        {
            return this.provIDField;
        }
        set
        {
            this.provIDField = value;
        }
    }

    /// <remarks/>
    public CardRegisterRequestReqType reqType
    {
        get
        {
            return this.reqTypeField;
        }
        set
        {
            this.reqTypeField = value;
        }
    }

    /// <remarks/>
    public string PAN
    {
        get
        {
            return this.pANField;
        }
        set
        {
            this.pANField = value;
        }
    }

    /// <remarks/>
    public string keyID
    {
        get
        {
            return this.keyIDField;
        }
        set
        {
            this.keyIDField = value;
        }
    }

    /// <remarks/>
    public string encryptPAN
    {
        get
        {
            return this.encryptPANField;
        }
        set
        {
            this.encryptPANField = value;
        }
    }

    /// <remarks/>
    public string consumerID
    {
        get
        {
            return this.consumerIDField;
        }
        set
        {
            this.consumerIDField = value;
        }
    }

    /// <remarks/>
    public string pubID
    {
        get
        {
            return this.pubIDField;
        }
        set
        {
            this.pubIDField = value;
        }
    }

    /// <remarks/>
    public string pubName
    {
        get
        {
            return this.pubNameField;
        }
        set
        {
            this.pubNameField = value;
        }
    }

    /// <remarks/>
    public CardRegisterRequestCoalitionList coalitionList
    {
        get
        {
            return this.coalitionListField;
        }
        set
        {
            this.coalitionListField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://api.offers.firstdata.com/v1/card")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public enum CardRegisterRequestReqType
{

    /// <remarks/>
    A,

    /// <remarks/>
    U,

    /// <remarks/>
    D,
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://api.offers.firstdata.com/v1/card")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class CardRegisterRequestCoalitionList
{

    private string[] addListField;

    private string[] removeListField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("coalitionID", IsNullable = false)]
    public string[] addList
    {
        get
        {
            return this.addListField;
        }
        set
        {
            this.addListField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("coalitionID", IsNullable = false)]
    public string[] removeList
    {
        get
        {
            return this.removeListField;
        }
        set
        {
            this.removeListField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://api.offers.firstdata.com/v1/card")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class CardRegisterResponse
{

    private string provIDField;

    private string reqTypeField;

    private string tokenField;

    private string tokenTypeField;

    private string cardSuffixField;

    private string consumerIDField;

    private string respCodeField;

    private string respTextField;

    private CardRegisterResponseCoalitionDetail[] errorCoalitionListField;

    /// <remarks/>
    public string provID
    {
        get
        {
            return this.provIDField;
        }
        set
        {
            this.provIDField = value;
        }
    }

    /// <remarks/>
    public string reqType
    {
        get
        {
            return this.reqTypeField;
        }
        set
        {
            this.reqTypeField = value;
        }
    }

    /// <remarks/>
    public string token
    {
        get
        {
            return this.tokenField;
        }
        set
        {
            this.tokenField = value;
        }
    }

    /// <remarks/>
    public string tokenType
    {
        get
        {
            return this.tokenTypeField;
        }
        set
        {
            this.tokenTypeField = value;
        }
    }

    /// <remarks/>
    public string cardSuffix
    {
        get
        {
            return this.cardSuffixField;
        }
        set
        {
            this.cardSuffixField = value;
        }
    }

    /// <remarks/>
    public string consumerID
    {
        get
        {
            return this.consumerIDField;
        }
        set
        {
            this.consumerIDField = value;
        }
    }

    /// <remarks/>
    public string respCode
    {
        get
        {
            return this.respCodeField;
        }
        set
        {
            this.respCodeField = value;
        }
    }

    /// <remarks/>
    public string respText
    {
        get
        {
            return this.respTextField;
        }
        set
        {
            this.respTextField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("CoalitionDetail", IsNullable = false)]
    public CardRegisterResponseCoalitionDetail[] errorCoalitionList
    {
        get
        {
            return this.errorCoalitionListField;
        }
        set
        {
            this.errorCoalitionListField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://api.offers.firstdata.com/v1/card")]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class CardRegisterResponseCoalitionDetail
{

    private string coalitionIDField;

    private string errorMsgField;

    /// <remarks/>
    public string coalitionID
    {
        get
        {
            return this.coalitionIDField;
        }
        set
        {
            this.coalitionIDField = value;
        }
    }

    /// <remarks/>
    public string ErrorMsg
    {
        get
        {
            return this.errorMsgField;
        }
        set
        {
            this.errorMsgField = value;
        }
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class CardRegisterRequest1
{

    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.offers.firstdata.com/v1/card", Order=0)]
    public CardRegisterRequest CardRegisterRequest;

    public CardRegisterRequest1()
    {
    }

    public CardRegisterRequest1(CardRegisterRequest CardRegisterRequest)
    {
        this.CardRegisterRequest = CardRegisterRequest;
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
[System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class CardRegisterResponse1
{

    [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.offers.firstdata.com/v1/card", Order=0)]
    public CardRegisterResponse CardRegisterResponse;

    public CardRegisterResponse1()
    {
    }

    public CardRegisterResponse1(CardRegisterResponse CardRegisterResponse)
    {
        this.CardRegisterResponse = CardRegisterResponse;
    }
}

[System.CodeDom.Compiler.GeneratedCode("", "")]
public interface registrationserviceChannel : registrationservice, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCode("", "")]
public partial class registrationserviceClient : System.ServiceModel.ClientBase<registrationservice>, registrationservice
{

    public registrationserviceClient()
    {
    }

    public registrationserviceClient(string endpointConfigurationName) :
        base(endpointConfigurationName)
    {
    }

    public registrationserviceClient(string endpointConfigurationName, string remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public registrationserviceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
        base(endpointConfigurationName, remoteAddress)
    {
    }

    public registrationserviceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
        base(binding, remoteAddress)
    {
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    OfferRegisterResponse1 registrationservice.OfferRegister(OfferRegisterRequest1 request)
    {
        return base.Channel.OfferRegister(request);
    }

    public OfferRegisterResponse OfferRegister(OfferRegisterRequest OfferRegisterRequest)
    {
        OfferRegisterRequest1 inValue = new OfferRegisterRequest1();
        inValue.OfferRegisterRequest = OfferRegisterRequest;
        OfferRegisterResponse1 retVal = ((registrationservice)(this)).OfferRegister(inValue);
        return retVal.OfferRegisterResponse;
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    System.Threading.Tasks.Task<OfferRegisterResponse1> registrationservice.OfferRegisterAsync(OfferRegisterRequest1 request)
    {
        return base.Channel.OfferRegisterAsync(request);
    }

    public System.Threading.Tasks.Task<OfferRegisterResponse1> OfferRegisterAsync(OfferRegisterRequest OfferRegisterRequest)
    {
        OfferRegisterRequest1 inValue = new OfferRegisterRequest1();
        inValue.OfferRegisterRequest = OfferRegisterRequest;
        return ((registrationservice)(this)).OfferRegisterAsync(inValue);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    CardRegisterResponse1 registrationservice.CardRegister(CardRegisterRequest1 request)
    {
        return base.Channel.CardRegister(request);
    }

    public CardRegisterResponse CardRegister(CardRegisterRequest CardRegisterRequest)
    {
        CardRegisterRequest1 inValue = new CardRegisterRequest1();
        inValue.CardRegisterRequest = CardRegisterRequest;
        CardRegisterResponse1 retVal = ((registrationservice)(this)).CardRegister(inValue);
        return retVal.CardRegisterResponse;
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    System.Threading.Tasks.Task<CardRegisterResponse1> registrationservice.CardRegisterAsync(CardRegisterRequest1 request)
    {
        return base.Channel.CardRegisterAsync(request);
    }

    public System.Threading.Tasks.Task<CardRegisterResponse1> CardRegisterAsync(CardRegisterRequest CardRegisterRequest)
    {
        CardRegisterRequest1 inValue = new CardRegisterRequest1();
        inValue.CardRegisterRequest = CardRegisterRequest;
        return ((registrationservice)(this)).CardRegisterAsync(inValue);
    }
}