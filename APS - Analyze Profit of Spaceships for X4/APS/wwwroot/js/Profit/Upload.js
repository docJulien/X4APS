$(document).ready(function() {
    $("#tabstrip").kendoTabStrip({
        animation: { open: { effects: "fadeIn" } }
    });
    $("#refreshData").click(function(e) {
        kendo.ui.progress($("body"), true);
        $.ajax({
            url: $("#urlRefreshData").val(),
            datatype: "json",
            data: {
                "filePath": $("#fileServerSide").val()
            },
            complete: function () {
                Upload.OnComplete();
                kendo.ui.progress($("body"), false);
            },
            success: function() {
                notification("Completed", "File Processed", "success");
            },
            error: errorHandler
        });
    });
});

var Upload = {
    OnComplete: function() {
        $("#pivotgrid").data("kendoPivotGrid").dataSource.read();
        $("#Grid").data("kendoGrid").dataSource.read();
    },
    ClearData: function () {
        var url = $('#urlClearData').data('url');
        kendo.ui.progress($("body"), true);
        $.ajax({
            url: url,
            //data: { },
            type: "POST",
            //dataType: 'json',
            success: function (result) {
                Upload.OnComplete();
                kendo.ui.progress($("body"), false);
                notification("Clear Data", "Data Erased", "info");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                notification("Clear Data", xhr.status + " " + thrownError, "error");
            }
        });
    }
};
