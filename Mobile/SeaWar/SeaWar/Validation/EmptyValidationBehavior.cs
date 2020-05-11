using Xamarin.Forms;

namespace SeaWar.Validation
{
    public class EmptyValidationBehavior: Behavior<Entry>
    {
        private Entry currentEntry;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            currentEntry = bindable;
            currentEntry.TextChanged += CurrentEntryOnTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            currentEntry.TextChanged -= CurrentEntryOnTextChanged;
            currentEntry = null;
        }
        
        private void CurrentEntryOnTextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }

        private void Validate()
        {
            var source = currentEntry.BindingContext as IUseValidation;
            source?.Validate();            
        }
    }
}