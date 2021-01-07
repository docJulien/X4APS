function getNomsGroupes(groupes) {
    var resultat = "";

    if (groupes === undefined || groupes === null || groupes.length === 0) {
        return resultat;
    }

    var j = groupes.length;
    for (var i = 0; i < j; i++) {
        resultat += "<span class='groupe'>" + groupes[i].Name + "</span>";
    }
    return resultat;
}
function userGridEdit(e) {
    // check si nouveau
    if (e.model.Id !== "") {
        // manipulate the edit form via jQuery
        $("#UserName").prop('disabled', true);
    }
}