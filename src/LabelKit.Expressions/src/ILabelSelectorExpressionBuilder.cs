// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

using System.Linq.Expressions;

/// <summary>
/// Instance transforming <see cref="ILabelSelector"/>s into expressions that can be used to filter <see cref="IQueryable"/>s.
/// </summary>
/// <remarks></remarks>
/// <remarks>For an overview of available builders, try <see cref="LabelSelectorExpressionBuilders"/>.</remarks>
/// <typeparam name="TLabels">Type of labels.</typeparam>
public interface ILabelSelectorExpressionBuilder<TLabels>
{
  /// <summary>
  /// Builds an expression from the specified <see cref="ILabelSelector"/>.
  /// </summary>
  /// <param name="selector">The selector.</param>
  /// <returns></returns>
  Expression<Func<TLabels, bool>> Build(ILabelSelector selector);
}
