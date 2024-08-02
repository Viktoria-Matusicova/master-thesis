namespace ruleService.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using ruleService.Consumers;
    using ruleService.Interfaces;
    using ruleService.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ParentRepository : IParentRepository
    {
        private readonly AppDbContext _appDbContext;

        public ParentRepository(AppDbContext appdbContext)
        {
            _appDbContext = appdbContext;
        }

        /// <summary>Modifies specified Parent rule.</summary>
        /// <param name="editRule"><see cref="Parent"/> rule to be modified.</param>
        /// <param name="value">Find rule from the database by value's ID.</param>
        /// <returns>Modified rule's ID.</returns>
        public async Task<string> EditParentRuleAsync(Rule editRule, string value = null)
        {
            Parent rule = value != null ? rule = this.GetParentRuleByValue(value):  _appDbContext.Parent.FirstOrDefault(x => x.Path == editRule.Path);
            if (rule == null) return null;
            try
            {
                rule.Path = editRule.Path;
                rule.Value = editRule.Data;
                await _appDbContext.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
            
            return rule.Id.ToString();
        }

        /// <summary>Finds Parent rule and sets it to deprecated.</summary>
        /// <param name="path">Path to the rule.</param>
        /// <returns>Modified rule's ID.</returns>
        public async Task<string> SetDeprecatedParentRuleAsync(string path)
        {
            var parentRule = _appDbContext.Parent.Where((rule) => rule.Path == path).FirstOrDefault();
            parentRule.IsDeprecated = true;
            await _appDbContext.SaveChangesAsync();

            return parentRule.Id.ToString();
        }

        /// <summary>Gets a user with the specified refresh token.</summary>
        /// <param name="id">The ID of the rule to retrieve.</param>
        /// <returns>The Parent rule with the specified ID, or null if not found.</returns>
        public Parent GetParentRuleById(Guid id)
        {
            return _appDbContext.Parent.Where(x => x.Id == id).FirstOrDefault();
        }
    }
}