﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.WinForms
{
	internal sealed class WinFormsExpressionSearch : ExpressionVisitor, IExpressionSearch
	{
		List<object> _results;
		Type _targetType;

		public List<T> FindObjects<T>(Expression expression) where T : class
		{
			_results = new List<object>();
			_targetType = typeof(T);
			Visit(expression);
			return _results.Select(o => o as T).ToList();
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression is ConstantExpression && node.Member is FieldInfo)
			{
				object container = ((ConstantExpression)node.Expression).Value;
				object value = ((FieldInfo)node.Member).GetValue(container);

				if (_targetType.IsInstanceOfType(value))
					_results.Add(value);
			}
			return base.VisitMember(node);
		}
	}
}
