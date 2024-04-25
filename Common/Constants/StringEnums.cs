namespace Common.Constants;

public static class StringEnum
{
    public static string ToValue(this Enum thisEnum)
    {
        string output = null;
        var type = thisEnum.GetType();

        var fieldInfo = type.GetField(thisEnum.ToString());
        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
            if (attrs != null && attrs.Length > 0)
            {
                output = attrs[0].Value;
            }
        }

        return output;
    }

    public static T GetValueFromStringValue<T>(string value) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(StringValue)) is StringValue attribute)
            {
                if (attribute.Value == value)
                {
                    return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == value)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }
        }

        throw new ArgumentException("Not Found", value);
    }

    private class StringValue : Attribute
    {
        public StringValue(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
    
    public enum AppErrorCode
    {
        Error,
        Warning,
        Info
    }
    
    public enum Roles
    {
        [StringValue("admin")] Admin,
        [StringValue("staff")] Staff,
        [StringValue("user")] User,
        [StringValue("customer")] Customer,
        [StringValue("storeOwner")] StoreOwner,
    }
    
    public enum Gender
    {
        [StringValue("Male")] Male,
        [StringValue("Female")] Female,
        [StringValue("Other")] Other,
    }
    
    public enum SellStatus
    {
        [StringValue("SoldOut")] SoldOut,
        [StringValue("Available")] Available,
    }
    
    public enum StallStatus
    {
        [StringValue("Deny")] Deny,
        [StringValue("Activate")] Activate,
        [StringValue("Pending")] Pending,
        [StringValue("Lock")] Lock,
    }
    
    public enum AddressStatus
    {
        [StringValue("NonDefault")] NonDefault, 
        [StringValue("Default")] Default,
    }
    public enum SelectedItemStatus
    {
        [StringValue("NonSelected")] NonSelected,
        [StringValue("Selected")] Selected
    }
    
    public enum ShippingStatus
    {
        [StringValue("Prepare")] Prepare,
        [StringValue("OnDelivery")] OnDelivery,
        [StringValue("Received")] Received 
    }

    public enum CategoryStatus
    {
       
        [StringValue("Lock")] Lock,
        [StringValue("Activate")] Activate,
        
    }
    
    public enum PaymentMethod
    {
        [StringValue("Cash")] Cash,
        [StringValue("Card")] Card
    }
    
    public enum PaymentStatus
    {
        [StringValue("success")] Success,
        [StringValue("RequiredMethod")] RequireMethod
    }
    
    public enum AccountStatus
    {
        [StringValue("Lock")] Lock,
        [StringValue("Active")] Active
    }

    public const string PAYMENT_SUCCESS = "success"; 
    public const string PAYMENT_REQUIRED_METHOD = "requires_payment_method"; 
    
}