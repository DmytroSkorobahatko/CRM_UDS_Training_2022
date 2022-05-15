var CreatePickupReport = {

    // CreatePickupReport.OpenForm

    OpenForm: function (primaryControl) {

        var entityFormOptions = {};

        entityFormOptions["entityName"] = "cr4de_cartransferreport";
        entityFormOptions["useQuickCreateForm"] = true;

        var formParameters = {};

        formParameters["cr4de_date"] = new Date();
        formParameters["cr4de_type"] = 420660000;

        // Open the form.

        Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
            function (success) {

                Xrm.Page.getAttribute("cr4de_pickupreport").setValue(success.savedEntityReference);
                Xrm.Page.getAttribute("cr4de_actualpickup").setValue(new Date());

            },
            function (error) {

                console.log(error);
            });

        
    }
}

var CreateReturnReport = {

    // CreateReturnReport.OpenRecord

    OpenForm: function (primaryControl) {

        var entityFormOptions = {};

        entityFormOptions["entityName"] = "cr4de_cartransferreport";
        entityFormOptions["useQuickCreateForm"] = true;

        var formParameters = {};

        formParameters["cr4de_date"] = new Date();
        formParameters["cr4de_type"] = 420660001;

        // Open the form.

        Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
            function (success) {

                Xrm.Page.getAttribute("cr4de_returnreport").setValue(success.savedEntityReference);
                Xrm.Page.getAttribute("cr4de_actualreturn").setValue(new Date());
                
            },
            function (error) {

                console.log(error);
            });

    }
}