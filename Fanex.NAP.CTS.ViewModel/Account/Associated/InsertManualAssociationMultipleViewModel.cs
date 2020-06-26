namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    public class InsertManualAssociationMultipleViewModel
    {
        public InsertManualAssociationMultipleViewModel()
        {
            ResultValidated = new ValidateManualAssociationMultipleViewModel();
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public ValidateManualAssociationMultipleViewModel ResultValidated { get; set; }

    }
}