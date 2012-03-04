using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace NProject.Web.Helpers
{
    public static class UIHelper
    {
        /// <summary>
        /// Creates a html element "select" from the specified enum.
        /// </summary>
        /// <typeparam name="TEnum">Type of enum which will be used.</typeparam>
        public static IEnumerable<SelectListItem> CreateSelectListFromEnum<TEnum>()
        {
            return UIHelper.CreateSelectListFromEnum(default(TEnum));
        }
        /// <summary>
        /// Creates a html element "select" from the specified enum and marks element as selected
        /// </summary>
        /// <typeparam name="TEnum">Type of enum which will be used.</typeparam>
        /// <param name="selectedValue">Value to select in output list.</param>
        public static IEnumerable<SelectListItem> CreateSelectListFromEnum<TEnum>(TEnum selectedValue)
        {
            var values = Enum.GetValues(typeof(TEnum));
            var names = Enum.GetNames(typeof(TEnum));
            int i = 0;
            var result = new List<SelectListItem>();
            foreach (TEnum v in values)
                result.Add(new SelectListItem { Text = names[i++], Value = i.ToString(), Selected = v.Equals(selectedValue) });

            return result;
        }
    }
}