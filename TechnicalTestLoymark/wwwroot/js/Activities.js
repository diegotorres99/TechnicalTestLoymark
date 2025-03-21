const uri = "/Activity/getActivities";

$(document).ready(() => {
    getActivities();
});
//$(function () {
//    getActivities();
//});
function getActivities() {
    fetch(uri)
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(dataJson => {
            $("#tbList tbody").empty(); 

            dataJson.forEach(item => {
                $("#tbList tbody").append($("<tr>").append(
                    $("<td>").text(formatDate(item.activityDate)), 
                    $("<td>").text(item.fullName),
                    $("<td>").text(item.activityDetail)
                ));
            });
        })
        .catch(error => {
            console.error("Error fetching data:", error);
        });
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString(); 
}