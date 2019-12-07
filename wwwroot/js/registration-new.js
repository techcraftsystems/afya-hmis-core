jq(function() {
    var patientTable;

    jq('#Patient_Person_Address_Telephone').formatter({
        'pattern': '+{{999}} {{999}} {{999999}}',
        'persistent': true
    });

    if (jq(".birthdate-picker").length > 0) {
        jq('.birthdate-picker').datepicker({
            format: 'mm/dd/yyyy',
            autoClose: true
        });
    }

    if ($(".users-edit").length > 0) {
        $("#patient-form").validate({
          rules: {
            DateOfBirth: {
              required: true,
              minlength: 1
            },
            'Patient.Person.Name': {
              required: true,
              minlength: 6
            },
            'Patient.PI.Identifier': {                
              required: true
            },
            'Patient.Person.Address.Email': {
              email: true
            },
            'Patient.Person.Address.Telephone': {
                required: true,
                minlength: 10
            }
          },
          errorElement: 'div'
        });
    }

    jq('#DateOfBirth').on('blur', function(){
        jq.ajax({
            dataType: "text",
            url: '/Registration/GetDateFromString',
            data: {
                "value": jq(this).val()
            },
            success: function(results) {
                jq('#DateOfBirth').val(results);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            }
        });
    });
});

function validateForm(){
    var count = jq('#Patient_Person_Name').val().split(' ').length;
    if (count < 2){
        jq('#Patient_Person_Name').addClass('error');
        M.toast({html: '<span>Specify atleast two Patient Names</span><a class="btn-flat red-text" href="#!">Close<a>'});
        return false;
    }

    return true;
}