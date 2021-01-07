// Pour utiliser les notification PNotify de style fontawesome au lieu de celles par défaut
$(function () {
    PNotify.prototype.options.styling = "fontawesome";    
});

// *** IMPORTANT ne pas supprimer *** Reload la grille après un enregistrement
function savePopup(e) {
    e.sender.one("dataBound", function () {
        e.sender.dataSource.read();
    });
}

function enableGridRow(e) {
    changeStatus(true, e);
}

function disableGridRow(e) {
    changeStatus(false, e);
}

function changeStatus(newstatus, e) {
    var grid = $(e.closest(".k-grid")).data("kendoGrid");
    var row = e.closest("tr");
    var model = grid.dataItem(row);
    model.set("Actif", newstatus);
    grid.dataSource.sync();
}

function saveHandler(s) {
    // Si Errors est non null, les messages de succès ne s'affichent pas, car il y'a des erreurs
    if (s.response === undefined || s.response.Errors !== null) {
        return;
    }

    // Pour les succès de mise à jour
    if (s.type === "update") {
        new PNotify({
            text: "Modification effectuée", // TODO utiliser les ressources files en JS
            type: "success",
            hide: true,         // Retirer la notification après un délai...
            delay: 8000,        // ... de 8 secondes
            mouse_reset: true,  // Reset le timer si la souris hover sur la notification
            animation: "fade"
        });
    }

    // Pour les succès de création
    if (s.type === "create") {
        new PNotify({
            text: "Création effectuée", // TODO utiliser les ressources files en JS
            type: "success",
            hide: true,         // Retirer la notification après un délai...
            delay: 8000,        // ... de 8 secondes
            mouse_reset: true,  // Reset le timer si la souris hover sur la notification
            animation: "fade"
        });
    }
}

function dataBound_Handler() {
}

//show server errors if any
function errorHandler(e) {
    console.log(e);
    var gridName = "Grid";
    var errortitle = "Oups... :/";
    var errormessage = "Le serveur a retourné une erreur anormale. ";
    if (e.xhr) {
        if (e.xhr.statusText) {
            errormessage = errormessage + " " + e.xhr.statusText;
        }
        if (e.xhr.status === 401) {
            errortitle = "Session Expirée...";
            errormessage = "Votre session a expiré. Vous serez redirigés dans 2 secondes...";
            setTimeout(function () {
                //redirect to login
                window.location.href = window.location.origin + "/Account/Login?ReturnUrl=" + window.location.pathname;
            }, 2000); //will call the function after 2 secs.

        }
    }
    var errortype = "error";
    if (e.errors) {
        errortitle = "Erreur de validation",   
        errormessage = e.errors;
        errortype = "warning";
    }
    if (e.errors || e.sender) {
        var grid = $('#' + gridName).data("kendoGrid");
        grid.one("dataBinding", function (p) {
            p.preventDefault();
        });
    }
    new PNotify({
        title: errortitle,    
        text: errormessage,
        type: errortype,  
        icon: true,    
        hide: true,          
        delay: 15000,        
        mouse_reset: true,   
        animation: "fade"
    });
}

// on ne recoit pas (e), mais (this)
function deleteRow_handler(element) {
    var grid = this;
    var target = element.toElement || element.target; // pour compatibilité avec browsers. .toElement fonctionne avec CHrome, .target avec IE
    
    var model = grid.dataSource.getByUid($(target).closest("tr").data("uid"));
    
    modalConfirmDialog("Désirez-vous effacer cette entrée?", "Effacer",
        function () {
            grid.removeRow($(target).closest("tr"));
            
            new PNotify({
                title: "Succès",
                text: "L'entrée a été effacée.",
                type: "success",
                icon: false,
                hide: true,          // Retirer la notification après un délai...
                delay: 10000,        // ... de 15 secondes
                mouse_reset: true,   // Reset le timer si la souris hover sur la notification
                animation: "fade"
            });
        }); 
};



// Fonction for notifications 
// different types: success, info, error, 
function notification(title, text, type) {
    new PNotify({
        title: title,
        text: text,
        type: type,
        styling: "fontawesome",
        animate: {
            animate: true,
            in_class: 'bounceInLeft',
            out_class: 'bounceOutRight'
        }
    });
};

// On pose une question et si oui, on exécute la fonction qui est passée en paramètre
// On passe en paramètre l'action à exécuter si on répond oui. Elle doit être dans une fonction anonyme si on ne désire pas qu'elle soit immédiatement exécutée.
// rappel: pour obtenir une fonction anonyme, on place: function(){ ... } comme dans l'exemple:  function () { grid.removeRow($(target).closest("tr")) })
function modalConfirmDialog(message, CommandText, functionToExecuteOnYes) {
    new PNotify({
        title: 'Veuillez Confirmer',
        text: message,
        icon: 'fa fa-hand-paper-o',
        hide: false,
        width: "400px",
        confirm: {
            buttons: [{
                text: CommandText, addClass: " btn-alert", promptTrigger: true,
                click: function (notice, value) {
                    functionToExecuteOnYes();
                    notice.remove();
                }
            },
            {
                text: "Annuler", addClass: "btn-primary", click: function (notice) {
                    notice.remove(); notice.get().trigger("pnotify.cancel", notice);
                    notification('Action annulée', '', 'info');
                }
            }],

            confirm: true
        },
        buttons: {

            closer: false,
            sticker: false,
        },
        history: {
            history: false
        },
        styling: "fontawesome",
        addclass: 'stack-modal',
        stack: { 'dir1': 'down', 'dir2': 'right', 'modal': true }
    });
};

function exportToExcel() {
    const grid = $('#Grid').data('kendoGrid');

    kendo.ui.progress(grid.element, true);
    var overlay = document.getElementsByClassName("k-loading-color");
    overlay[0].style.opacity = 1;

    //Code pour envoyer TOUTES les colonnes même cachées sur excel. On les révèle et les recache après.
    //var columns = grid.columns;

    //if (columns.length > 0) {
    //    for (var i = 0; i < columns.length; i++) {
    //        if (columns[i].hidden)
    //            grid.showColumn(i);
    //    }
    //}

    grid.saveAsExcel();
};
function exportToPDF(url) {
    alert(url);
};

// use this function for the "exportExcel" callback in the grid configuration

function resetGridAfterExport(e) {
    const grid = $('#Grid').data('kendoGrid');

    var columns = grid.columns;

    if (columns.length > 0) {
        for (var i = 0; i < columns.length; i++) {
            if (columns[i].hidden === false)
                grid.hideColumn(i);
        }
    }

    kendo.ui.progress(grid.element, false);
};

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function GridReload() {
    $("#Grid").data("kendoGrid").dataSource.read();
};

function NumericFilter(args) {
    $(args).kendoNumericTextBox({
        "spinners": false,
        "format": "n0",
        "decimals": 0,
    });
}

function GoToMenu() {
    window.location.href = '/';
}


function StripTimeFromDate(d) {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

function openInNewTab(url) {
    var win = window.open(url, '_blank');
    win.focus();
}



var noHint = $.noop;

function placeholder(element) {
    return element.clone().addClass("k-state-hover").css("opacity", 0.65);
}

function onChangeOrder(e) {

    var url = $('#maj_Order').data('url');
    var grid = $("#Grid").data("kendoGrid"),
        skip = grid.dataSource.skip(),
        oldIndex = e.oldIndex + skip,
        newIndex = e.newIndex + skip,
        data = grid.dataSource.data(),
        dataItem = grid.dataSource.getByUid(e.item.data("uid"));

    grid.dataSource.remove(dataItem);
    grid.dataSource.insert(newIndex, dataItem);
    
    $.ajax({
        url: url,
        data: { Id: dataItem.Id, newOrder: newIndex },
        type: "POST",
        //dataType: 'json',
        success: function (result) {
            notification("Ordre","L'ordre a changé","info")
        },
        error: function (xhr, ajaxOptions, thrownError) {
            notification("Ordre", xhr.status + " " + thrownError, "error");
        }
    });
};

function saveGridState (e) {
    var grid = $("#Grid").data("kendoGrid");
    localStorage[e+"-grid-options"] = kendo.stringify(grid.getOptions());
};
function loadGridState(e) {
    var grid = $("#Grid").data("kendoGrid");
    var options = localStorage[e +"-grid-options"];
    if (options) {
        grid.setOptions(JSON.parse(options));
    }
};
function loadDefaultGridState(e) {
    var grid = $("#Grid").data("kendoGrid");
    grid.setOptions(defaultgridoptions);
};


function RedirectToReport(Adress) {
    var recordid = $('#editorCurrentId').val();
    var url = Adress.replace("TOREPLACE", recordid);

    openInNewTab(url);
}
