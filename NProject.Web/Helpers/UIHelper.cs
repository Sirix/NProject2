using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

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

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor(htmlHelper, expression, null);
        }
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            //http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                values.Select(value => new SelectListItem
                {
                    Text = value.ToString(),
                    Value = value.ToString(),
                    Selected = value.Equals(metadata.Model)
                });

            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }
     
        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string controlName, object htmlAttributes = null)
        {
            IEnumerable<TEnum> values = Enum.GetValues(typeof (TEnum)).Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                values.Select(value => new SelectListItem
                                           {
                                               Text = value.ToString(),
                                               Value = value.ToString(),
                                               //Selected = value.Equals(selectedValue)
                                           });

            return htmlHelper.DropDownList(controlName, items, htmlAttributes);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            //http://weblogs.asp.net/imranbaloch/archive/2010/07/03/asp-net-mvc-labelfor-helper-with-htmlattributes.aspx
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.SetInnerText(labelText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Generates a fully qualified URL to an action method by using
        /// the specified action name, controller name and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteAction(this UrlHelper url,
            string actionName, string controllerName, object routeValues = null)
        {
            string scheme = url.RequestContext.HttpContext.Request.Url.Scheme;

            return url.Action(actionName, controllerName, routeValues, scheme);
        }
    }
}