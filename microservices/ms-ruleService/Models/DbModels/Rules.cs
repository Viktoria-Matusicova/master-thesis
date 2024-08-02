namespace ruleService.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>Represents a custom rule in the application, which contains information about the associated parent, SIEMs, Sigma, and user.</summary>
    public class Rules
    {
        /// <summary>The unique identifier of the rule.</summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>The foreign key referencing the <see cref="Parent"/>.</summary>
        [ForeignKey("Parent")]
        public Guid ParentId { get; set; }

        /// <summary>A list of SIEMs associated with this rule.</summary>
        public List<Siem> Siems { get; set; }

        /// <summary>The foreign key referencing the <see cref="Sigma"/>.</summary>
        [ForeignKey("Sigma")]
        public Guid SigmaId { get; set; }

        /// <summary>The <see cref="Sigma"/> associated with this rule.</summary>
        public Sigma Sigma { get; set; }

        /// <summary>The ID of the user who created this rule.</summary>
        public Guid UserId { get; set; }
    }
}