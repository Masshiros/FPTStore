// load cart
function cart() {
    $.ajax({
        type: 'GET',
        url: `/customer/cart/getcart`,
        dataType: 'json',
        success: function (response) {
            // console.log(response);

            var content = "";
            $.each(response.data.shoppingCartList, function (key, value) {
                content += `<div class="row border-bottom pb-3">
                        <div class="d-none d-lg-block col-lg-1 text-center py-2">
                            ${value.product.imageUrl != null ? ` <img src="${value.product.imageUrl}" class="card-img-top rounded w-100" />` : `<img src="https://placehold.co/500x600/png" class="card-img-top rounded w-100" />`}
                        </div>
                        <div class="col-12 col-lg-6 pt-md-3">
                            <h5 class="text-uppercase text-secondary"><strong>${value.product.productTitle}</strong></h5>
                            <p><small>${value.product.productDescription} </small></p>
                        </div>
                        <div class="col-12  col-lg-5 text-center row">
                            <div class="col-3 text-md-right pt-2 pt-md-4">
                                <h6 class="fw-semibold">
                                   ${value.price.toLocaleString('vi-VN', {
                                       style: 'currency',
                                       currency: 'VND'
                                   })}
                                    <span class="text-muted">&nbsp;x&nbsp;</span>${value.count}
                                </h6>
                            </div>
                            <div class="col-6 col-sm-4 col-lg-6 pt-2">
                                <div class="w-75 btn-group" role="group">
                                    <a onclick="cartIncrement(${value.shoppingCartId})" class="btn btn-outline-primary bg-gradient py-2">
                                        <i class="bi bi-plus-square"></i>
                                    </a> &nbsp;
                                    <a onclick="cartDecrement(${value.shoppingCartId})" class="btn btn-outline-primary bg-gradient py-2">
                                        <i class="bi bi-dash-square"></i>
                                    </a>
                                </div>

                            </div>
                            <div class="col-3 col-sm-4 col-lg-2 offset-lg-1 text-right pt-2">
                                <a onclick="cartRemove(${value.shoppingCartId})" class="btn btn-danger bg-gradient py-2 ">
                                    <i class="bi bi-trash-fill"></i>
                                </a>
                            </div>
                        </div>

                    </div>`
            });
            var cartTotal = `<h5 class="text-dark fw-semibold text-uppercase"> Total (VND)</h5>
                             <h4 class="text-dark fw-bolder">${response.data.orderTotal.toLocaleString('vi-VN', {
                                 style: 'currency',
                                 currency: 'VND'
                             })} </h4>`
            $('#cartPage').html(content);
            $(`#cartTotal`).html(cartTotal);
        }
    })
}


// remove cart
function cartRemove(cartId) {
    $.ajax({
        type: "GET",
        url: `cart/remove?cartId=${cartId}`,
        dataType: `json`,
        success: function (data) {
            cart();
        }
    })
}
// cart increment
function cartIncrement(cartId) {
    $.ajax({
        type: "GET",
        url: `cart/plus?cartId=${cartId}`,
        dataType: `json`,
        success: function (data) {
            cart();
        }
    })
}
// cart decrement
function cartDecrement(cartId) {
    $.ajax({
        type: "GET",
        url: `cart/minus?cartId=${cartId}`,
        dataType: `json`,
        success: function (data) {
            cart();
        }
    })
}

cart();