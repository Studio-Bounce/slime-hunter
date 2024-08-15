using UnityEngine.UIElements;

public class TabbedMenuController
{
    /* Define member variables*/
    private const string tabClassName = "tab";
    private const string selectedTabClassName = "selected-tab";
    private const string selectedContentClassName = "selected-content";
    // Tab and tab content have the same prefix but different suffix
    // Define the suffix of the tab name
    private const string tabNameSuffix = "Tab";
    // Define the suffix of the tab content name
    private const string contentNameSuffix = "Content";
    private readonly VisualElement root;

    public TabbedMenuController(VisualElement root)
    {
        this.root = root;
    }

    public void RegisterTabCallbacks()
    {
        UQueryBuilder<VisualElement> tabs = GetAllTabs();
        tabs.ForEach((VisualElement tab) => {
            tab.RegisterCallback<ClickEvent>(TabOnClick);
        });
    }

    public void SwitchTab(string tabName)
    {
        VisualElement clickedTab = root.Query<VisualElement>(name: tabName);
        if (!TabIsCurrentlySelected(clickedTab))
        {
            GetAllTabs().Where(
                (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
            ).ForEach(UnselectTab);
            SelectTab(clickedTab);
        }
    }

    public string CurrentTab()
    {
        foreach (var tab in GetAllTabs().ToList()) {
            if (TabIsCurrentlySelected(tab))
            {
                return tab.name;
            }
        }
        return "";
    }

    private void TabOnClick(ClickEvent evt)
    {
        VisualElement clickedTab = evt.currentTarget as VisualElement;
        if (!TabIsCurrentlySelected(clickedTab))
        {
            GetAllTabs().Where(
                (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
            ).ForEach(UnselectTab);
            SelectTab(clickedTab);
        }
    }

    // Method that returns a Boolean indicating whether a tab is currently selected
    private static bool TabIsCurrentlySelected(VisualElement tab)
    {
        return tab.ClassListContains(selectedTabClassName);
    }

    private UQueryBuilder<VisualElement> GetAllTabs()
    {
        return root.Query<VisualElement>(className: tabClassName);
    }

    private void SelectTab(VisualElement tab)
    {
        tab.AddToClassList(selectedTabClassName);
        VisualElement content = FindContent(tab);
        content.AddToClassList(selectedContentClassName);
    }

    private void UnselectTab(VisualElement tab)
    {
        tab.RemoveFromClassList(selectedTabClassName);
        VisualElement content = FindContent(tab);
        content.RemoveFromClassList(selectedContentClassName);
    }

    // Method to generate the associated tab content name by for the given tab name
    private static string GenerateContentName(VisualElement tab) =>
        tab.name.Replace(tabNameSuffix, contentNameSuffix);

    // Method that takes a tab as a parameter and returns the associated content element
    private VisualElement FindContent(VisualElement tab)
    {
        return root.Q(GenerateContentName(tab));
    }
}