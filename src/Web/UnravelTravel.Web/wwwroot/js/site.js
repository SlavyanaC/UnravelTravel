function getSearchResult() {
    let destinationId = $("#DestinationId").val();
    let startDate = $("#StartDate").val();
    let endDate = $("#EndDate").val();

    $.ajax({
        type: "GET",
        url: `/Search?DestinationId=${destinationId}&StartDate=${startDate}&EndDate=${endDate}`,
        success: function (res) {
            $("#searchResult").remove();
            $("#searchContainer").append(res);
        },
        error: function (res) {
            console.log(res);
        }
    });
}

function reviewRestaurant(restaurantId) {
    let rating = +$(`input[name=${restaurantId}-rating]:checked`).val();
    let content = $(`#${restaurantId}-content`).val();
    let url = "/Restaurants/Review";

    let data = { restaurantId: restaurantId, rating: rating, content: content };
    $.ajax({
        type: "GET",
        url: url,
        data: data,
        success: function (res) {
            $(`#${restaurantId}-button`).attr("disabled", "true").text("Reviewed");
            $(`#${restaurantId}-reviewModal`).modal("toggle");
        },
        error: function (err) {
            console.log(err);
        }
    });
}