/// <summary>
/// Purpose:
/// This file contains assembly-level attributes used by WPF for theme and
/// resource lookup. The ThemeInfo attribute instructs WPF where to search for
/// theme specific and generic resource dictionaries at runtime.
/// </summary>

using System.Windows;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            // where theme specific resource dictionaries are located
                                                // (used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   // where the generic resource dictionary is located
                                                // (used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]
