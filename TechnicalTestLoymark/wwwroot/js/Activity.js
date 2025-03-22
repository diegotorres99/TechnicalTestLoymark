const uri = "/Activity/";

$(function () {
    getActivities();
});
function getActivities() {
    fetch(uri + 'getActivities')
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(dataJson => {
            $("#tbList tbody").empty();
            console.log(dataJson)
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