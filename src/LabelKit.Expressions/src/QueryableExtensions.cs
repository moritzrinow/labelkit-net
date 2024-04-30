// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

using System.Linq.Expressions;
using LinqKit;

public static class QueryableExtensions
{
  /// <inheritdoc cref="MatchLabels{T,TLabels}(System.Linq.IQueryable{T},System.Linq.Expressions.Expression{System.Func{T,TLabels}},LabelKit.ILabelSelector?,LabelKit.ILabelSelectorExpressionBuilder{TLabels})"/>
  public static IQueryable<T> MatchLabels<T, TLabels>(this IQueryable<T> query,
    Expression<Func<T, TLabels>> labelExpression,
    Func<LabelSelector, LabelSelector> selector,
    ILabelSelectorExpressionBuilder<TLabels> expressionBuilder)
    => query.MatchLabels(labelExpression, selector(LabelSelector.New()), expressionBuilder);

  /// <inheritdoc cref="MatchLabels{T,TLabels}(System.Linq.IQueryable{T},System.Linq.Expressions.Expression{System.Func{T,TLabels}},LabelKit.ILabelSelector?,LabelKit.ILabelSelectorExpressionBuilder{TLabels})"/>
  public static IQueryable<T> MatchLabels<T, TLabels>(this IQueryable<T> query,
    Func<LabelSelector, LabelSelector> selector,
    ILabelSelectorExpressionBuilder<TLabels> expressionBuilder)
    where T : ILabelledEntity<TLabels>
    => query.MatchLabels(e => e.ArrayLabels, selector, expressionBuilder);


  /// <summary>
  /// Applies the specified <see cref="LabelSelector"/> to the <see cref="IQueryable{T}"/> using the expression provided by the <see cref="ILabelSelectorExpressionBuilder{TLabels}"/>.
  /// </summary>
  /// <param name="query">The query to filter.</param>
  /// <param name="labelExpression">The labels to match.</param>
  /// <param name="selector">The selector.</param>
  /// <param name="expressionBuilder">The expression-builder.</param>
  /// <typeparam name="T">Type of the entities.</typeparam>
  /// <typeparam name="TLabels">Type of the labels.</typeparam>
  /// <returns>The filtered query.</returns>
  public static IQueryable<T> MatchLabels<T, TLabels>(this IQueryable<T> query,
    Expression<Func<T, TLabels>> labelExpression,
    ILabelSelector? selector,
    ILabelSelectorExpressionBuilder<TLabels> expressionBuilder)
  {
    if (selector is null)
    {
      return query;
    }

    var selectorExpression = expressionBuilder.Build(selector);

    Expression<Func<T, bool>> expression = e => selectorExpression.Invoke(labelExpression.Invoke(e));

    return query.Where(expression.Expand());
  }

  /// <inheritdoc cref="MatchLabels{T,TLabels}(System.Linq.IQueryable{T},System.Linq.Expressions.Expression{System.Func{T,TLabels}},LabelKit.ILabelSelector?,LabelKit.ILabelSelectorExpressionBuilder{TLabels})"/>
  public static IQueryable<T> MatchLabels<T, TLabels>(this IQueryable<T> query,
    ILabelSelector? selector,
    ILabelSelectorExpressionBuilder<TLabels> expressionBuilder)
    where T : ILabelledEntity<TLabels>
    => query.MatchLabels(e => e.ArrayLabels, selector, expressionBuilder);
}
