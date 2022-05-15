function CarClassOnChange() {
    Xrm.Page.getAttribute("cr4de_car").setValue(null);

    Xrm.Page.getControl("cr4de_car").addPreSearch(function () {
        addCarFilter();
    });
}
function addCarFilter() {
    var carclass = Xrm.Page.getAttribute("cr4de_carclass").getValue();
    var fetchXml = "";
    
    if (carclass)
        fetchXml = "<filter type='and'><condition attribute='cr4de_car_class' value='" + carclass[0].id + "' operator='eq'/></filter>";

    Xrm.Page.getControl("cr4de_car").addCustomFilter(fetchXml);
}

function ReservedpickupOnChange() {
    // Reserved pickup date/time cannot be earlier than current date

    var pickupDate = Xrm.Page.getAttribute("cr4de_reservedpickup").getValue();
    var today = new Date();
    var errorMessage = "Reserved pickup date cannot be earlier than current date";

    if (Date.parse(pickupDate) < Date.parse(today))
        Xrm.Page.getControl("cr4de_reservedpickup").setNotification(errorMessage, "wrong_reservedpickup_date");
    else
        Xrm.Page.getControl("cr4de_reservedpickup").clearNotification("wrong_reservedpickup_date");
}

function PriceOnSave() {
    // Price should be calculated automatically based on next formula:
    //Car class.Price * Difference in Days(End date / time – Start date / time) + 100(if pickup location is not office) + 100(if return location is not office)

    var pickupLocation = Xrm.Page.getAttribute("cr4de_pickuplocation").getText();
    var returnLocation = Xrm.Page.getAttribute("cr4de_returnlocation").getText();
    var pickupDate = Xrm.Page.getAttribute("cr4de_reservedpickup").getValue();
    var handoverDate = Xrm.Page.getAttribute("cr4de_reservedhandover").getValue();
    var carclassid = Xrm.Page.getAttribute("cr4de_carclass").getValue()[0].id;

    var oneDay = 1000 * 60 * 60 * 24;
    var differenceInDays;
    var price;

    Xrm.WebApi.retrieveRecord("cr4de_carclass", carclassid, "?$select=cr4de_price").then(
        function success(result) {

            differenceInDays = ((handoverDate.getTime() - pickupDate.getTime()) / oneDay);
            price = Number(result.cr4de_price) * differenceInDays;

            if (pickupLocation != null && pickupLocation != "Office")
                price += 100;
            if (returnLocation != null && returnLocation != "Office") 
                price += 100;
            
            Xrm.Page.getControl("cr4de_price").setDisabled(false);
            Xrm.Page.getAttribute("cr4de_price").setValue(price);
            Xrm.Page.getControl("cr4de_price").setDisabled(true);

        },
        function error(error) {
            alert(error.message)
        });
}
