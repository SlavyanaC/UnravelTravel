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

function loadTableBookForm(restaurantId) {
    $.ajax({
        url: `/Reservations/BookPartial`,
        data: { id: restaurantId },
        success: function (res) {
            $("#actionsContainer").empty();
            $("#actionsContainer").append(res);
        },
        error: function (res) {
            console.log(res);
        }
    });
}

function bookTable(restaurantId) {
    let guestUserEmail = $('#email').val();
    let date = $('#date').val();
    let peopleCount = $('#peopleCount').val();

    let data = { restaurantId: restaurantId, guestUserEmail: guestUserEmail, date: date, peopleCount: peopleCount };
    $.ajax({
        url: "/Reservations/Book",
        data: data,
        success: function (res) {
            $("#actionsContainer").empty();
            let bookButton =
                `<btn id="bookTableBtn" onclick="loadTableBookForm(${restaurantId})" class="btn btn-lg btn-outline-primary">Book table</btn>`;
            $("#actionsContainer").append(bookButton);
            alert("Successfully book your table. Please check your email for confirmation.");
        },
        error: function (res) {
            console.log(res);
        }
    });
}