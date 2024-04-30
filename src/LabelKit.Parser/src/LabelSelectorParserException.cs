// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

using Pidgin;

/// <summary>
/// Exception representing an error during parsing of raw label-selectors.
/// </summary>
/// <param name="error">The parse error.</param>
public class LabelSelectorParserException(ParseError<LabelSelectorToken>? error)
  : Exception(error?.RenderErrorMessage())
{
  /// <summary>
  /// Specific parse error.
  /// </summary>
  public ParseError<LabelSelectorToken>? Error { get; } = error;
}
