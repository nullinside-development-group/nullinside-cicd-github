﻿namespace Nullinside.Cicd.GitHub;

/// <summary>
///   Constants used throughout the appliction.
/// </summary>
public static class Constants {
  /// <summary>
  ///   The github project name
  /// </summary>
  public const string GITHUB_ORG = "nullinside-development-group";

  /// <summary>
  ///   The url to the graphql endpoint to post against.
  /// </summary>
  public const string GITHUB_GRAPHQL_URL = "https://api.github.com/graphql";

  /// <summary>
  ///   The name of the project that be sent in user agent headers.
  /// </summary>
  public const string PROJECT_NAME = "nullinside-cicd-github";

  /// <summary>
  ///   The github project's unique identifier on github.
  /// </summary>
  public const int GITHUB_PROJECT_NUM = 1;

  /// <summary>
  ///   The unique identifier on GitHub for the "done" status of items.
  /// </summary>
  /// <remarks>This was found by manually querying the API. It's a nightmare to pull. Hence the hardcoding.</remarks>
  public const string GITHUB_PROJECT_DONE_SINGLE_SELECT_ID = "98236657";
}