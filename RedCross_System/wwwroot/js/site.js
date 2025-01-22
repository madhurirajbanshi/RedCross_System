// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const controllerUrls = {
  'home': '/Home',
  'branch': '/Branch',
  'user': '/User',
  'donor': '/Donor',
  'campaign': '/Campaign',
  'donation': '/Donation',
  'bloodstock': '/BloodStock',
  'testblood': '/TestBlood'
};

document.addEventListener('DOMContentLoaded', function () {
  const searchForm = document.querySelector('.form-inline');
  const searchInput = searchForm.querySelector('input[type="search"]');

  // Create and append datalist for autocomplete
  const datalist = document.createElement('datalist');
  datalist.id = 'controller-suggestions';
  Object.keys(controllerUrls).forEach(term => {
    const option = document.createElement('option');
    option.value = term.charAt(0).toUpperCase() + term.slice(1);
    datalist.appendChild(option);
  });
  document.body.appendChild(datalist);

  // Add datalist to input
  searchInput.setAttribute('list', 'controller-suggestions');

  searchForm.addEventListener('submit', function (e) {
    e.preventDefault();
    const searchTerm = searchInput.value.toLowerCase().trim();

    if (controllerUrls[searchTerm]) {
      window.location.href = controllerUrls[searchTerm];
    } else {
      // Show error message if the search term doesn't match any controller
      const alertDiv = document.createElement('div');
      alertDiv.className = 'alert alert-warning alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3';
      alertDiv.style.zIndex = '1050';
      alertDiv.innerHTML = `
                <strong>Not found!</strong> No matching page found for "${searchInput.value}".
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            `;
      document.body.appendChild(alertDiv);

      // Remove the alert after 3 seconds
      setTimeout(() => {
        alertDiv.remove();
      }, 3000);
    }

    // Clear the search input
    searchInput.value = '';
  });
});