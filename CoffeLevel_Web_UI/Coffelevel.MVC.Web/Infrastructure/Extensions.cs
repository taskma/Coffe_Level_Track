using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AutoMapper;
using System.Linq.Expressions;
using CoffeLevel.Data.Entities;
using CoffeLevel.Client.Web.Models;
using System.Globalization;
using System.Text;

namespace CoffeLevel.Client.Web.Infrastructure
{
    public static class Extensions
    {
        public static IEnumerable<SelectListItem> ToSelectList<TEntity>(this IEnumerable<TEntity> list, Func<TEntity, string> text, Func<TEntity, string> value, string EmptyText)
        {

            var retList = list.Select(p => new SelectListItem
                                               {
                                                   Value = value(p),
                                                   Text = text(p)
                                               }).OrderBy(p => p.Text).ToList();

            if(!string.IsNullOrEmpty(EmptyText))
                retList.Insert(0, new SelectListItem { Text = EmptyText, Value = string.Empty });
            
            return retList;
        }

        public static IEnumerable<SelectListItem> ToSelectListWithValue<TEntity>(this IEnumerable<TEntity> list, Func<TEntity, string> text, Func<TEntity, string> value, string defaultValue, string allValue)
        {

            var retList = list.Select(p => new SelectListItem
            {
                Value = value(p),
                Text = text(p),
                Selected = text(p).ToLower() == defaultValue.ToLower() ? true : false
            }).ToList();
            if (!string.IsNullOrEmpty(allValue))
                retList.Insert(0, new SelectListItem { Text = allValue, Value = string.Empty, Selected = false });

            return retList;
        }

        public static string toTitleCase(this string word)
        {
            char? lastValue = null;
            StringBuilder builder = new StringBuilder();
            foreach (var item in word.ToCharArray())
            {
                bool toUpper = (lastValue.HasValue && lastValue.Value == ' ') || !lastValue.HasValue;

                    switch (item)
                    {
                        case 'ş':
                        case 'Ş':
                            builder.Append(toUpper ? 'Ş' : 'ş');
                            break;
                        case 'ç':
                        case 'Ç':
                            builder.Append(toUpper ? 'Ç' : 'ç');
                            break;
                        case 'ö':
                        case 'Ö':
                            builder.Append(toUpper ? 'Ö' : 'ö');
                            break;
                        case 'ü':
                        case 'Ü':
                            builder.Append(toUpper ? 'Ü' : 'ü');
                            break;
                        case 'ğ':
                        case 'Ğ':
                            builder.Append(toUpper ? 'Ğ' : 'ğ');
                            break;
                        case 'ı':
                        case 'I':
                            builder.Append(toUpper ? 'I' : 'ı');
                            break;
                        case 'i':
                        case 'İ':
                            builder.Append(toUpper ? 'İ' : 'i');
                            break;
                        default:
                            builder.Append(toUpper ? item.ToString().ToUpper() : item.ToString().ToLower());
                            break;
                    }     
                lastValue = item;
                
            }
            return builder.ToString();
           //return new System.Globalization.CultureInfo("tr-TR").TextInfo.ToTitleCase(word.ToLower());
           //return CultureInfo.CurrentCulture. TextInfo.ToTitleCase(word.ToLower());
        }

        public static  IEnumerable<SelectListItem> ToSelectList<TEntity>(this IQueryable<TEntity> list )
        {
            throw new NotImplementedException();
        } 

        public static TEntity ModelToEntityMapper<TModel, TEntity>(TModel model, TEntity entity)
            where TModel : BaseCEModel
            where TEntity : Entity
        {
            Mapper.CreateMap<TModel, TEntity>().ForMember("Id", p => p.Ignore());
            return Mapper.Map<TModel, TEntity>(model, entity);
        }

        public static TModel EntityToModelMapper<TEntity, TModel>(TEntity entity, TModel model)
            where TModel : BaseCEModel
            where TEntity : Entity
        {
            Mapper.CreateMap<TEntity, TModel>();
            return Mapper.Map<TEntity, TModel>(entity, model);
        }
    }
}