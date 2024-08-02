namespace ruleService.Models
{
    /// <summary>Represents a ViewModel of the custom rule.</summary>
    public class RuleViewModel
    {
        /// <summary>The ID of the associated custom rule.</summary>
        public string RuleId { get; set; }

        /// <summary>The list of the associated Siems.</summary>
        public List<SiemViewModel> Siems { get; set; } = null;

        /// <summary>The <see cref="SigmaViewModel"/> associated with this rule.</summary>
        public SigmaViewModel Sigma { get; set; }
    }

    /// <summary>Represents a ViewModel of the <see cref="Sigma"/> object.</summary>
    public class SigmaViewModel
    {
        /// <summary>The <see cref="FrameworkViewModel"/> associated with this Sigma object.</summary>
        public FrameworkViewModel Framework { get; set; }

        /// <summary>The current value of the Sigma object.</summary>
        public string Value { get; set; }
    }

    /// <summary>Represents a ViewModel of the <see cref="Framework"/> object.</summary>
    public class FrameworkViewModel
    {
        /// <summary>The name of the framework.</summary>
        public string Name { get; set; }

        /// <summary>The generated name of the custom rule.</summary>
        public string GeneratedName { get; set; }

        /// <summary>The category of the custom rule.</summary>
        public string Category { get; set; }

        /// <summary>The subcategory of the custom rule.</summary>
        public string Subcategory { get; set; }

        /// <summary>The reference link of the custom rule.</summary>
        public string Reference { get; set; }
    }
}