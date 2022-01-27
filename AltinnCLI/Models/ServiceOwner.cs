using System.Collections.Generic;

namespace AltinnCLI.Models
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10", IsNullable = false)]
    public class ServiceOwner
    {

        private string serviceOwnerNameField;

        private ServiceOwnerSubscription subscriptionField;

        private ServiceOwnerPrefill prefillField;

        /// <remarks/>
        public string ServiceOwnerName
        {
            get
            {
                return this.serviceOwnerNameField;
            }
            set
            {
                this.serviceOwnerNameField = value;
            }
        }

        /// <remarks/>
        public ServiceOwnerSubscription Subscription
        {
            get
            {
                return this.subscriptionField;
            }
            set
            {
                this.subscriptionField = value;
            }
        }

        /// <remarks/>
        public ServiceOwnerPrefill Prefill
        {
            get
            {
                return this.prefillField;
            }
            set
            {
                this.prefillField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerSubscription
    {

        private string sequenceNoField;

        private string externalShipmentReferenceField;

        private ServiceOwnerSubscriptionReportee[] reporteeField;

        /// <remarks/>
        public string SequenceNo
        {
            get
            {
                return this.sequenceNoField;
            }
            set
            {
                this.sequenceNoField = value;
            }
        }

        /// <remarks/>
        public string ExternalShipmentReference
        {
            get
            {
                return this.externalShipmentReferenceField;
            }
            set
            {
                this.externalShipmentReferenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Reportee")]
        public ServiceOwnerSubscriptionReportee[] Reportee
        {
            get
            {
                return this.reporteeField;
            }
            set
            {
                this.reporteeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerSubscriptionReportee
    {

        private string idField;

        private ServiceOwnerSubscriptionReporteeFormTask[] formTaskField;

        /// <remarks/>
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FormTask")]
        public ServiceOwnerSubscriptionReporteeFormTask[] FormTask
        {
            get
            {
                return this.formTaskField;
            }
            set
            {
                this.formTaskField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerSubscriptionReporteeFormTask
    {

        private string externalServiceCodeField;

        private string externalServiceEditionCodeField;

        private System.DateTime startDateField;

        private System.Nullable<System.DateTime> expirationDateField;

        private bool expirationDateFieldSpecified;

        private System.DateTime nextScheduledDateField;

        private System.DateTime nextDueDateField;

        private System.DateTime visibleDateField;

        private bool visibleDateFieldSpecified;

        private string periodTypeField;

        private int caseIdField;

        private bool caseIdFieldSpecified;

        private IdentifyingFieldsIdentifyingField[] identifyingFieldsField;

        /// <remarks/>
        public string ExternalServiceCode
        {
            get
            {
                return this.externalServiceCodeField;
            }
            set
            {
                this.externalServiceCodeField = value;
            }
        }

        /// <remarks/>
        public string ExternalServiceEditionCode
        {
            get
            {
                return this.externalServiceEditionCodeField;
            }
            set
            {
                this.externalServiceEditionCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime StartDate
        {
            get
            {
                return this.startDateField;
            }
            set
            {
                this.startDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> ExpirationDate
        {
            get
            {
                return this.expirationDateField;
            }
            set
            {
                this.expirationDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ExpirationDateSpecified
        {
            get
            {
                return this.expirationDateFieldSpecified;
            }
            set
            {
                this.expirationDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime NextScheduledDate
        {
            get
            {
                return this.nextScheduledDateField;
            }
            set
            {
                this.nextScheduledDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime NextDueDate
        {
            get
            {
                return this.nextDueDateField;
            }
            set
            {
                this.nextDueDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime VisibleDate
        {
            get
            {
                return this.visibleDateField;
            }
            set
            {
                this.visibleDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool VisibleDateSpecified
        {
            get
            {
                return this.visibleDateFieldSpecified;
            }
            set
            {
                this.visibleDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string PeriodType
        {
            get
            {
                return this.periodTypeField;
            }
            set
            {
                this.periodTypeField = value;
            }
        }

        /// <remarks/>
        public int CaseId
        {
            get
            {
                return this.caseIdField;
            }
            set
            {
                this.caseIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CaseIdSpecified
        {
            get
            {
                return this.caseIdFieldSpecified;
            }
            set
            {
                this.caseIdFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("IdentifyingField", IsNullable = false)]
        public IdentifyingFieldsIdentifyingField[] IdentifyingFields
        {
            get
            {
                return this.identifyingFieldsField;
            }
            set
            {
                this.identifyingFieldsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class IdentifyingFieldsIdentifyingField
    {

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerPrefill
    {

        private string sequenceNoField;

        private string externalShipmentReferenceField;

        private List<ServiceOwnerPrefillReportee> reporteeField;

        /// <remarks/>
        public string SequenceNo
        {
            get
            {
                return this.sequenceNoField;
            }
            set
            {
                this.sequenceNoField = value;
            }
        }

        /// <remarks/>
        public string ExternalShipmentReference
        {
            get
            {
                return this.externalShipmentReferenceField;
            }
            set
            {
                this.externalShipmentReferenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Reportee")]
        public List<ServiceOwnerPrefillReportee> Reportee
        {
            get
            {
                return this.reporteeField;
            }
            set
            {
                this.reporteeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerPrefillReportee
    {

        private string idField;

        private ServiceOwnerPrefillReporteeFormTask[] formTaskField;

        private ServiceOwnerPrefillReporteeField[] fieldField;

        /// <remarks/>
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FormTask")]
        public ServiceOwnerPrefillReporteeFormTask[] FormTask
        {
            get
            {
                return this.formTaskField;
            }
            set
            {
                this.formTaskField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Field")]
        public ServiceOwnerPrefillReporteeField[] Field
        {
            get
            {
                return this.fieldField;
            }
            set
            {
                this.fieldField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerPrefillReporteeFormTask
    {

        private string externalServiceCodeField;

        private string externalServiceEditionCodeField;

        private System.DateTime validFromField;

        private System.DateTime validToField;

        private string identityFieldHashCodeField;

        private string sendersReferenceField;

        private string receiversReferenceField;

        private IdentifyingFieldsIdentifyingField[] identifyingFieldsField;

        private ServiceOwnerPrefillReporteeFormTaskAttachment[] attachmentsField;

        private ServiceOwnerPrefillReporteeFormTaskForm[] formField;

        /// <remarks/>
        public string ExternalServiceCode
        {
            get
            {
                return this.externalServiceCodeField;
            }
            set
            {
                this.externalServiceCodeField = value;
            }
        }

        /// <remarks/>
        public string ExternalServiceEditionCode
        {
            get
            {
                return this.externalServiceEditionCodeField;
            }
            set
            {
                this.externalServiceEditionCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ValidFrom
        {
            get
            {
                return this.validFromField;
            }
            set
            {
                this.validFromField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ValidTo
        {
            get
            {
                return this.validToField;
            }
            set
            {
                this.validToField = value;
            }
        }

        /// <remarks/>
        public string IdentityFieldHashCode
        {
            get
            {
                return this.identityFieldHashCodeField;
            }
            set
            {
                this.identityFieldHashCodeField = value;
            }
        }

        /// <remarks/>
        public string SendersReference
        {
            get
            {
                return this.sendersReferenceField;
            }
            set
            {
                this.sendersReferenceField = value;
            }
        }

        /// <remarks/>
        public string ReceiversReference
        {
            get
            {
                return this.receiversReferenceField;
            }
            set
            {
                this.receiversReferenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("IdentifyingField", IsNullable = false)]
        public IdentifyingFieldsIdentifyingField[] IdentifyingFields
        {
            get
            {
                return this.identifyingFieldsField;
            }
            set
            {
                this.identifyingFieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Attachment", IsNullable = false)]
        public ServiceOwnerPrefillReporteeFormTaskAttachment[] Attachments
        {
            get
            {
                return this.attachmentsField;
            }
            set
            {
                this.attachmentsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Form")]
        public ServiceOwnerPrefillReporteeFormTaskForm[] Form
        {
            get
            {
                return this.formField;
            }
            set
            {
                this.formField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerPrefillReporteeFormTaskAttachment
    {
        private string attachmentNameField;

        private string fileNameField;

        private byte[] attachmentDataField;

        private string sendersReferenceField;

        private bool encryptedField;

        private string attachmentTypeField;

        private bool signingLockedField;

        private bool signingLockedFieldSpecified;

        private bool signedByDefaultField;

        private bool signedByDefaultFieldSpecified;

        /// <remarks/>
        public string AttachmentName
        {
            get
            {
                return this.attachmentNameField;
            }
            set
            {
                this.attachmentNameField = value;
            }
        }

        /// <remarks/>
        public string FileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] AttachmentData
        {
            get
            {
                return this.attachmentDataField;
            }
            set
            {
                this.attachmentDataField = value;
            }
        }

        /// <remarks/>
        public string SendersReference
        {
            get
            {
                return this.sendersReferenceField;
            }
            set
            {
                this.sendersReferenceField = value;
            }
        }

        /// <remarks/>
        public bool Encrypted
        {
            get
            {
                return this.encryptedField;
            }
            set
            {
                this.encryptedField = value;
            }
        }

        /// <remarks/>
        public string AttachmentType
        {
            get
            {
                return this.attachmentTypeField;
            }
            set
            {
                this.attachmentTypeField = value;
            }
        }

        /// <remarks/>
        public bool SigningLocked
        {
            get
            {
                return this.signingLockedField;
            }
            set
            {
                this.signingLockedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SigningLockedSpecified
        {
            get
            {
                return this.signingLockedFieldSpecified;
            }
            set
            {
                this.signingLockedFieldSpecified = value;
            }
        }

        /// <remarks/>
        public bool SignedByDefault
        {
            get
            {
                return this.signedByDefaultField;
            }
            set
            {
                this.signedByDefaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SignedByDefaultSpecified
        {
            get
            {
                return this.signedByDefaultFieldSpecified;
            }
            set
            {
                this.signedByDefaultFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerPrefillReporteeFormTaskForm
    {

        private string dataFormatIdField;

        private string dataFormatVersionField;

        private string sendersReferenceField;

        private string parentReferenceField;

        private string formDataField;

        private bool signingLockedField;

        private bool signingLockedFieldSpecified;

        private bool signedByDefaultField;

        private bool signedByDefaultFieldSpecified;

        /// <remarks/>
        public string DataFormatId
        {
            get
            {
                return this.dataFormatIdField;
            }
            set
            {
                this.dataFormatIdField = value;
            }
        }

        /// <remarks/>
        public string DataFormatVersion
        {
            get
            {
                return this.dataFormatVersionField;
            }
            set
            {
                this.dataFormatVersionField = value;
            }
        }

        /// <remarks/>
        public string SendersReference
        {
            get
            {
                return this.sendersReferenceField;
            }
            set
            {
                this.sendersReferenceField = value;
            }
        }

        /// <remarks/>
        public string ParentReference
        {
            get
            {
                return this.parentReferenceField;
            }
            set
            {
                this.parentReferenceField = value;
            }
        }

        /// <remarks/>
        public string FormData
        {
            get
            {
                return this.formDataField;
            }
            set
            {
                this.formDataField = value;
            }
        }

        /// <remarks/>
        public bool SigningLocked
        {
            get
            {
                return this.signingLockedField;
            }
            set
            {
                this.signingLockedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SigningLockedSpecified
        {
            get
            {
                return this.signingLockedFieldSpecified;
            }
            set
            {
                this.signingLockedFieldSpecified = value;
            }
        }

        /// <remarks/>
        public bool SignedByDefault
        {
            get
            {
                return this.signedByDefaultField;
            }
            set
            {
                this.signedByDefaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SignedByDefaultSpecified
        {
            get
            {
                return this.signedByDefaultFieldSpecified;
            }
            set
            {
                this.signedByDefaultFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    public class ServiceOwnerPrefillReporteeField
    {

        private string idField;

        private System.DateTime validFromField;

        private System.DateTime validToField;

        private int indexField;

        private bool indexFieldSpecified;

        private string valueField;

        /// <remarks/>
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ValidFrom
        {
            get
            {
                return this.validFromField;
            }
            set
            {
                this.validFromField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ValidTo
        {
            get
            {
                return this.validToField;
            }
            set
            {
                this.validToField = value;
            }
        }

        /// <remarks/>
        public int Index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IndexSpecified
        {
            get
            {
                return this.indexFieldSpecified;
            }
            set
            {
                this.indexFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.altinn.no/services/intermediary/serviceinitiation/2009/10", IsNullable = false)]
    public class IdentifyingFields
    {

        private IdentifyingFieldsIdentifyingField[] identifyingFieldField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("IdentifyingField")]
        public IdentifyingFieldsIdentifyingField[] IdentifyingField
        {
            get
            {
                return this.identifyingFieldField;
            }
            set
            {
                this.identifyingFieldField = value;
            }
        }
    }
}


