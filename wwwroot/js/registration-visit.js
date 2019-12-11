jq(function() {
    String.prototype.toAccounting = function() {
        var str = parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
        if (str.charAt(0) == '-') {
            return '(' + str.substring(1, 40) + ')';
        }
        else {
            return str;
        }
    };

    jq('#Queue_Room_Type_Id').change(function(){
        GetRoomsIEnumerable();
    });

    jq('#Queue_Room_Type_Id, #Queue_Room_Id, #Visit_ClientCode_Id').change(function(){
        var clnt_code = jq('#Visit_ClientCode_Id').val();
        var room_type = jq('#Queue_Room_Type_Id').val();
        var room_idnt = jq('#Queue_Room_Id').val();

        if (room_type <= 2){
            GetClientCodeBillableRate(clnt_code, 1, room_type);
        }
        else {
            //With specialization
        }
    });

    jq(".select2").select2({
        dropdownAutoWidth: true,
        width: '100%'
    });

    jq(".max-length").select2({
        dropdownAutoWidth: true,
        width: '100%',
        maximumSelectionLength: 10,
        placeholder: "Select Doctors"
    });

    jq('#Doctor').change(function(){
        jq('#DoctorString').val(jq('#Doctor').val().toString());
        //console.log(jq('#DoctorString').val(jq('#Doctor').val().toString()));
    });
    
    jq('#Visit_ClientCode_Id').change();
});

function validateForm(){
    jq('#DoctorString').val(jq('#Doctor').val().toString());
    if (jq('#Referral_Type_Id').val() != "") {
        if (jq('#Referral_Facility').val() == "" && jq('#Doctor').val().length == 0){
            M.toast({html: '<span>Specify the referring doctor and/or Facility</span><a class="btn-flat red-text" href="#!">Close<a>'});
            return false;
        }

        if (jq('#Referral_Notes').val() == ""){
            M.toast({html: '<span>Specify the referral details</span><a class="btn-flat red-text" href="#!">Close<a>'});
            return false;
        }
    }

    var count = jq('#Waiver:checked').length;
    if (count >= 1 && jq('#WaiverReason').val().trim() == ""){
        M.toast({html: '<span>You must specify reason for Waiving</span><a class="btn-flat red-text" href="#!">Close<a>'});
        return false;
    }

    return true;
}

function GetRoomsIEnumerable() {
    var types = jq('#Queue_Room_Id');
    types.find('option').remove();
    types.append('<option value="">Loading..</option>');
    types.formSelect();

    jq.ajax({
        dataType: "json",
        url: '/Registration/GetRoomsIEnumerable',
        data: {
            "type": jq("#Queue_Room_Type_Id").val()
        },
        success: function(results) {
            types.find('option').remove();
            jq.each(results, function(i, opt) {
                types.append('<option value="' + opt.value + '">' + opt.text + '</option>');
            });
        },
        error: function(xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function() {
            types.formSelect();
        }
    }); 
}

function GetClientCodeBillableRate(code, service) {
    jq.ajax({
        dataType: "json",
        url: '/Registration/GetClientCodeBillableRate',
        data: {
            "code": code,
            "service": service
        },
        success: function(results) {
            jq('#Bill_Amount').val(results.amount.toString().toAccounting());
            jq('#Item_Amount').val(results.amount);
            jq('#Item_Service_Id').val(results.service.id);
        },
        error: function(xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        }
    }); 
}