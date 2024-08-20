using Microsoft.AspNetCore.Components.Forms;

namespace SuggestionSiteUI.Components
{
    public class CustomRadioGroup<T> : InputRadioGroup<T>
    {
        string _name, _fieldClass;

        protected override void OnParametersSet()
        {
            var fieldClass = EditContext?.FieldCssClass(FieldIdentifier) ?? "";
            if (fieldClass != _fieldClass || Name != _name) 
            {
                _fieldClass = fieldClass;
                _name = Name;
                base.OnParametersSet();
            }
        }
    }
}
