// Global variables for pagination state
let currentPage = 1;
let totalPages = 1;
let pageSize = 10;
let totalRecords = 0;
let requestStartTime = null;
let requestTimer = null;

$(document).ready(function () {
    // Load initial roster data
    loadRosterData(1, 10);
    checkNetworkSpeed();

    // Save roster on form submit
    $("#rosterForm").on("submit", function (e) {
        e.preventDefault();
        saveRoster();
    });

    // Pagination click the datefrom to works 
    $(document).on("click", ".pagination a", function (e) {
        e.preventDefault();

        const page = $(this).data("page");
        const dateFroms = $("#dateFroms").val();
        const dateTos = $("#dateTos").val();
        const search = $("#rosterSearch").val();

        loadRosterData(page, 10, search, dateFroms, dateTos, $("#nameFilter").val(), $("#shiftFilter").val());
    });


    // Clear function
    $("#clearRosterBtn").on("click", function () {
        $("#rosterForm")[0].reset();
        $("#employeeListBody input[type='checkbox']").prop("checked", false);
    });

    // Change entries per page - using your specific dropdown ID
    $("#entriesPage").on("change", function () {
        const newSize = parseInt($(this).val());
        changePageSize(newSize,$("#dateFrom").val(), $("#dateTo").val());
    });

    // Bind pagination button events using the specific IDs from your HTML
    $("#firstPage").on('click', function (e) {
        e.preventDefault();
        if (!$(this).parent().hasClass('disabled')) {
            goToFirstPage();
        }
    });

    $("#prevPage").on('click', function (e) {
        e.preventDefault();
        if (!$(this).parent().hasClass('disabled')) {
            goToPreviousPage();
        }
    });

    $("#nextPage").on('click', function (e) {
        e.preventDefault();
        if (!$(this).parent().hasClass('disabled')) {
            goToNextPage();
        }
    });

    $("#lastPage").on('click', function (e) {
        e.preventDefault();
        if (!$(this).parent().hasClass('disabled')) {
            goToLastPage();
        }
    });

    // Handle direct page number clicks ()
    $(".pagination").on('click', 'a[data-page]', function (e) {
        e.preventDefault();
        if (!$(this).parent().hasClass('active')) {
            const page = parseInt($(this).data('page'));
            loadRosterData(page, pageSize, $("#rosterSearch").val(), $("#dateFroms").val(), $("#dateTos").val(), $("#nameFilter").val(), $("#shiftFilter").val());
        }
    });
});

// Search Functionality with 2s debounce system  (2000ms = 2 seconds
let searchTimeout;
$("#rosterSearch").on('keyup', function () {
    clearTimeout(searchTimeout); // cancel previous timer
    let searchValue = $(this).val().toLowerCase();

    searchTimeout = setTimeout(function () {
        loadRosterData(1, 10, searchValue, $("#dateFroms").val(), $("#dateTos").val(), $("#nameFilter").val(), $("#shiftFilter").val());
    }, 1000); 
});

// Auto filter using Date From and Date To
$("#dateFroms, #dateTos").on("change", function () {
    const dateFrom = $("#dateFroms").val();
    const dateTo = $("#dateTos").val();
    const search = $("#rosterSearch").val();

    loadRosterData(1, 10, search, dateFrom, dateTo, $("#nameFilter").val(), $("#shiftFilter").val(), ("#nameFilter").val(), $("#shiftFilter").val());
});

//Auto filter Name wise "Name Filter"
$("#nameFilter").on("change", function () {
    const name = $(this).val();
    const shift = $("#shiftFilter").val();
    const dateFrom = $("#dateFroms").val();
    const dateTo = $("#dateTos").val();
    const search = $("#rosterSearch").val();
    loadRosterData(1, 10, search, dateFrom, dateTo, name, shift);
}); 

//Auto filter ShiftF wise "Shift Filter"
$("#shiftFilter").on("change", function () {
    const name = $("#nameFilter").val();
    const shift = $(this).val();
    const dateFrom = $("#dateFroms").val();
    const dateTo = $("#dateTos").val();
    const search = $("#rosterSearch").val();
    loadRosterData(1, 10, search, dateFrom, dateTo, name, shift);

})


// Load roster data with pagination
let isLoading = false;
function loadRosterData(page = 1, size = 10, search = "", dateFrom = null, dateTo = null, name = "", shiftF = "" ) {
    //currentPage = page;
    //pageSize = size;

    if (isLoading) {
        return; // Prevent multiple simultaneous loads
    }
    isLoading = true;
    currentPage = page;
    pageSize = size;

    //Show loading indicator
    showLoadingIndicator();

    const df = dateFrom || null;    
    const dt = dateTo || null;
    console.log(`Loading data for Page: ${page}, Size: ${size}, Search: '${search}', DateFrom: '${df}', DateTo: '${dt}' `);

   
    $.ajax({
        url: "/Roster/GetAll",
        type: "POST",
        timeout: 20000, // 10 seconds timeout
        data: {
            page: page,
            pageSize: size,
            search: search,
            dateFrom: df,
            dateTo: dt,
            name: name,
            shift: shiftF
            
        },
        success: function (res) {
            if (res.success) {
                renderRosterTable(res.data);
                updatePaginationInfo(res.pagination);
                updatePaginationControls(res.pagination);
                renderPagination(res.pagination)
            } else {
                console.log("Error: " + res.message);
                alert("Error loading data: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            hideLoadingIndicator();

            if (status === 'timeout') {
                alert("Request timed out. The system may be under heavy load. Please try again.");
            } else {
                alert("Failed to load roster data! " + error);
            }
            console.error("AJAX Error:", {
                status: status,
                error: error,
                response: xhr.responseText
            });
            //show error message in table
            $("#rosterTableBody").html(`
                <tr>
                    <td colspan="10" class="text-center text-danger">
                        <i class="bi bi-exclamation-triangle"></i>
                        Failed to load data. Please try again.
                    </td>
                </tr>
            `);
        },
        complete: function () {
            hideLoadingIndicator();
            isLoading = false;
        }
    });
}

//Live Indicator functions
function showLoadingIndicator() {
    requestStartTime = Date.now();
    $('#loadingOverlay').addClass('show'); // ✅ Fixed selector

    requestTimer = setInterval(() => {
        const elapsed = ((Date.now() - requestStartTime) / 1000).toFixed(1);
        $('#requestTime').text(`${elapsed}s`);
    }, 100);

    $("#firstPage, #prevPage, #nextPage, #lastPage, .pagination a[data-page]").addClass('disabled');
    $("#entriesPage").prop('disabled', true);
    $("#nameFilter, #shiftFilter, #dateFroms, #dateTos, #rosterSearch").prop('disabled', true);

    $("#rosterTableBody").html(`
        <tr>
            <td colspan="10" class="text-center">
                <div class="spinner-border spinner-border-sm text-primary" role="status">
                    <span class="visually-hidden">Loading roster data...</span>
                </div>
                Loading data...
            </td>
        </tr>
    `);
}

// Hide loading indicator
function hideLoadingIndicator() {
    clearInterval(requestTimer);
    const totalTime = ((Date.now() - requestStartTime) / 1000).toFixed(2);
    console.log(`Request completed in ${totalTime}s`);

    setTimeout(() => {
        $('#loadingOverlay').removeClass('show'); // ✅ Corrected: remove "show"
    }, 200);

    $("#entriesPage").prop('disabled', false);
    $("#nameFilter, #shiftFilter, #dateFroms, #dateTos, #rosterSearch").prop('disabled', false);
}

//Check Network Speed (Optional)
function checkNetworkSpeed() {
    const startTime = Date.now();

    $.ajax({
        url: "/Roster/GetAll",
        type: "POST",
        data: { page: 1, pageSize: 1 },
        success: function () {
            const loadTime = Date.now() - startTime;
            console.log(`Network response time: ${loadTime}ms`);

            if (loadTime > 3000) {
                console.warn("Slow network detected!");
                // You can show a warning to user
            }
        }
    });
}


// Render roster data table
function renderRosterTable(data) {
    let html = "";
    console.log(data);
    if (data && data.length > 0) {
        data.forEach(item => {
            const date = new Date(item.Date).toLocaleDateString();
            const day = new Date(item.Date).toLocaleDateString("en-US", { weekday: "long" });

            html += `
                <tr>
                    <td>${item.aI_ID || item.SlNo}</td>
                    <td>${item.employeeID || ""}</td>
                    <td>${item.employeeName || ""}</td>
                    <td>${item.designationName || ""}</td>
                    <td>${item.date}</td>
                    <td>${item.roseterScheduleCode || ''}</td>
                    <td>${item.shiftName || ""}</td>
                    <td>${item.shiftStartTime || ""}</td>
                    <td>${item.shiftEndTime || ""}</td>
                    <td>
                        <button class="btn btn-sm btn-primary" onclick="editRoster(${item.aI_ID})">Edit</button>
                        <button class="btn btn-sm btn-danger" onclick="deleteRoster(${item.aI_ID})">Delete</button>
                    </td>
                </tr>
            `;
        });
    } else {
        html = `
            <tr>
                <td colspan="10" class="text-center">No data available</td>
            </tr>
        `;
    }

    $("#rosterTableBody").html(html);
}

//Render Pagination
function renderPagination(pagination, totalPages, currentPage, search, dateFroms, dateTos) {
    let container = $("#paginationContainer");
    container.empty();

    if (pagination.totalPages > 1) {
        if (pagination.hasPrevious) {
            container.append(`<button class="btn btn-sm btn-primary" onclick="loadRosterData(${pagination.currentPage - 1}, ${pagination.pageSize}, '${search}', '${dateFroms}', '${dateTos}')">Previous</button>`);
        }
        container.append(`<span class="mx-2">Page ${pagination.currentPage} of ${pagination.totalPages}</span>`);
        if (pagination.hasNext) {
            container.append(`<button class="btn btn-sm btn-primary" onclick="loadRosterData(${pagination.currentPage + 1}, ${pagination.pageSize}, '${search}', '${dateFroms}', '${dateTos}')">Next</button>`);
        }
    }
}

// Update pagination information text
function updatePaginationInfo(pagination) {
    totalPages = pagination.totalPages;
    totalRecords = pagination.totalRecords;

    const start = ((pagination.currentPage - 1) * pagination.pageSize) + 1;
    const end = Math.min(pagination.currentPage * pagination.pageSize, pagination.totalRecords);

    // Update the "Showing X to Y of Z entries" text
    $(".text-muted.small").text(
        `Showing ${start} to ${end} of ${pagination.totalRecords} entries`
    );
}

// Update pagination controls (buttons)
function updatePaginationControls(pagination) {
    // Update First button
    if (pagination.hasPrevious) {
        $("#firstPage").parent().removeClass('disabled');
        $("#firstPage").removeClass('disabled');
    } else {
        $("#firstPage").parent().addClass('disabled');
        $("#firstPage").addClass('disabled');
    }

    // Update Previous button
    if (pagination.hasPrevious) {
        $("#prevPage").parent().removeClass('disabled');
        $("#prevPage").removeClass('disabled');
    } else {
        $("#prevPage").parent().addClass('disabled');
        $("#prevPage").addClass('disabled');
    }

    // Update Next button
    if (pagination.hasNext) {
        $("#nextPage").parent().removeClass('disabled');
        $("#nextPage").removeClass('disabled');
    } else {
        $("#nextPage").parent().addClass('disabled');
        $("#nextPage").addClass('disabled');
    }

    // Update Last button
    if (pagination.hasNext) {
        $("#lastPage").parent().removeClass('disabled');
        $("#lastPage").removeClass('disabled');
    } else {
        $("#lastPage").parent().addClass('disabled');
        $("#lastPage").attr('href', '#').addClass('disabled');
    }

    // Generate page numbers
    generatePageNumbers(pagination);
}

// Generate page number buttons
function generatePageNumbers(pagination) {
    const maxVisiblePages = 5;
    let startPage = Math.max(1, pagination.currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(pagination.totalPages, startPage + maxVisiblePages - 1);

    // Adjust start page if we're near the end
    if (endPage - startPage < maxVisiblePages - 1) {
        startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    // Remove existing page number buttons (keep First, Previous, Next, Last)
    $(".pagination li:not(:first):not(:nth-child(2)):not(:nth-last-child(2)):not(:last)").remove();

    // Insert page numbers between Previous and Next buttons
    let pageNumbersHtml = "";
    for (let i = startPage; i <= endPage; i++) {
        const activeClass = i === pagination.currentPage ? 'active' : '';
        pageNumbersHtml += `
            <li class="page-item ${activeClass}">
                <a class="page-link" href="#" data-page="${i}">${i}</a>
            </li>
        `;
    }

    // Insert the page numbers before the Next button
    $("#nextPage").parent().before(pageNumbersHtml);
}

// Attach click handlers to page numbers with filters (Temporarry)
$(".pagination a[data-page]").off('click').on('click', function (e) {
    e.preventDefault();
    if (!$(this).parent().hasClass('active')) {
        const page = parseInt($(this).data('page'));
        loadRosterData(page, pageSize, search, dateFrom, dateTo);
    }
});

// Pagination button click handlers
function goToFirstPage() {
    if (currentPage > 1) {
        loadRosterData(1, pageSize, $("#rosterSearch").val(), $("#dateFroms").val(), $("#dateTos").val(), $("#nameFilter").val(), $("#shiftFilter").val());

    }
}

function goToPreviousPage() {
    if (currentPage > 1) {
        loadRosterData(currentPage - 1, pageSize, $("#rosterSearch").val(), $("#dateFroms").val(), $("#dateTos").val(), $("#nameFilter").val(), $("#shiftFilter").val());
    }

    }


function goToNextPage() {
    if (currentPage < totalPages) {
        loadRosterData(currentPage + 1, pageSize, $("#rosterSearch").val(), $("#dateFroms").val(), $("#dateTos").val(), $("#nameFilter").val(), $("#shiftFilter").val());
    }
}

function goToLastPage() {
    if (currentPage < totalPages) {
        loadRosterData(totalPages, pageSize, $("#rosterSearch").val(), $("#dateFroms").val(), $("#dateTos").val().$("#nameFilter").val(). $("#shiftFilter").val());
    }
}

// Change page size
function changePageSize(newSize) {
    pageSize = newSize;
    loadRosterData(1, newSize, $("#rosterSearch").val(), $("#dateFroms").val(), $("#dateTos").val(), $("#nameFilter").val(), $("#shiftFilter").val()); // Go back to first page when changing page size
}

// Save Roster function (enhanced from your original)
function saveRoster() {
    const dateFrom = $("#dateFrom").val();
    const dateTo = $("#dateTo").val();
    const shift = $("#shift").val();
    const selectedEmployees = [];

    $("input[name='employeeSelect']:checked").each(function () {
        selectedEmployees.push($(this).val());
    });

    if (!dateFrom || !dateTo || selectedEmployees.length === 0) {
        alert("Please fill all required fields!");
        return;
    }

    const payload = {
        DateFrom: dateFrom,
        DateTo: dateTo,
        Shift: shift,
        Employees: selectedEmployees
    };

    // AJAX call to save roster
    $.ajax({
        url: "/Roster/Save",
        type: "POST",
        data: payload,
        success: function (res) {
            if (res.success) {
                alert("Roster saved successfully!");
                // Reload the roster data to show new entries
                loadRosterData(currentPage, pageSize);
                // Clear the form
                $("#rosterForm")[0].reset();
                $("#employeeListBody input[type='checkbox']").prop("checked", false);
            } else {
                alert("Error: " + res.message);
            }
        },
        error: function () {
            alert("Failed to save roster!");
        }
    });

    console.log("Saving roster:", payload);
}

// Edit and Delete functions
function editRoster(id) {
    // Implement edit functionality
    console.log("Edit roster with ID:", id);
    // You can open a modal or redirect to edit page
    // Example: window.location.href = `/Roster/Edit/${id}`;
}

function deleteRoster(id) {
    if (confirm("Are you sure you want to delete this roster entry?")) {
        $.ajax({
            url: "/Roster/Delete",
            type: "POST",
            data: { id: id },
            success: function (res) {
                if (res.success) {
                    alert("Roster entry deleted successfully!");
                    loadRosterData(currentPage, pageSize); // Reload current page
                } else {
                    alert("Error: " + res.message);
                }
            },
            error: function () {
                alert("Failed to delete roster entry!");
            }

        });
    }
}
