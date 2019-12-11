jq(function() {
    var patientTable;

    jq('#patient-age').formatter({
        'pattern': '{{999}}',
        'persistent': true
    });

    if (jq("#users-list-datatable").length > 0) {
        patientTable = $("#users-list-datatable").DataTable({
          responsive: true,
          'columnDefs': [{
            "orderable": false,
            "targets": [0,6,7,8]
          }],
          "displayLength": 25
        });
    };

    jq('a.btn-floating.btn-large').click(function(){
        var redirect = "?name=" + jq('#patient-names').val().trim();
        if (jq('#patient-identification').val().trim() != ''){
            redirect += "&id=" + jq('#patient-identification').val().trim();
        }
        if (jq('#patient-contact').val().trim() != ''){
            redirect += "&phone=" + jq('#patient-contact').val().trim();
        }
        if (jq('#patient-age').val().trim() != ''){
            redirect += "&age=" + jq('#patient-age').val().trim();
        }
        if (jq('#patient-gender').val().trim() != ''){
            redirect += "&gender=" + jq('#patient-gender').val().trim();
        }

        window.location.href = "/registration/new" + redirect;
    });

    jq('a.btn-search').click(function(){
        SearchPatients();
    });

    jq('div.users-list-table').hide();
});

function SearchPatients() {
    jq.ajax({
        dataType: "json",
        url: '/Registration/SearchPatients',
        data: {
            "names":        jq("#patient-names").val(),
            "identifier":   jq("#patient-identification").val(),
            "phone":        jq("#patient-contact").val(),
            "age":          jq("#patient-age").val(),
            "gender":       jq("#patient-gender").val(),
            "visit":        jq("#patient-visit").val()
        },
        success: function(results) {
            var tbl = $('#users-list-datatable').DataTable();
            tbl.clear().draw();
            
            
            //jq('#users-list-datatable tbody').empty();
            jq.each(results, function(i, pt) {
                var lastVisit = (pt.lastVisit == "01/01/1900" ? "Never" : pt.lastVisit);
                var iStatus = "";
                var iIcons = '&nbsp; <a href="/registration/edit?=' + pt.uuid + '"><i class="material-icons">edit</i></a><a href="/registration/visit?p=' + pt.uuid + '"> <i class="material-icons">add_to_queue</i></a>';

                if (pt.status.id == 0){
                    iStatus = '<span class="chip amber lighten-5"><span class="amber-text">' + pt.status.name + '</span> </span>';
                }
                else if (pt.status.id == 1){
                    iStatus = '<span class="chip green lighten-5"><span class="green-text">' + pt.status.name + '</span> </span>';
                }
                else if (pt.status.id == 2){
                    iStatus = '<span class="chip orange lighten-5"><span class="orange-text">' + pt.status.name + '</span> </span>';
                }
                else if (pt.status.id == 3){
                    iStatus = '<span class="chip gray lighten-5"><span class="gray-text">' + pt.status.name + '</span> </span>';
                }
                else if (pt.status.id == 99){
                    iStatus = '<span class="chip red lighten-5"><span class="red-text">' + pt.status.name + '</span> </span>';
                }
                else {
                    iStatus = '<span class="chip blue lighten-5"><span class="blue-text">' + pt.status.name + '</span> </span>';
                }

                tbl.row.add( [
                    pt.identifier,
                    "<a class='blue-text' href='/patients/" + pt.uuid + "'>" + pt.person.name + "</a>",
                    pt.age,
                    (pt.person.gender == "m" ? "Male" : "Female"),
                    pt.person.address.telephone,
                    pt.person.address.location,
                    lastVisit,
                    iStatus,
                    iIcons
                ] ).draw( false );
            })
        },
        error: function(xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function() {
            jq('div.users-list-table').show();
            jq('div.fixed-action-btn').removeClass('hide');
        }
    }); 
}