const uri = "/User/"; 
let isEditing = false;
let currentItem = null;

const userModel = {
    id: 0,
    name: "",
    lastName: "",
    email: "",
    birthDate: new Date(),
    phone: "",
    country: "",
    contactQuestion: false
};

$(function () {
    getUsers();
    populateCountries();
});
function getUsers() {
    fetch(uri + "getUsers")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(data => {
            $("#tbList tbody").empty();
            data.forEach(user => {
                $("#tbList tbody").append($("<tr>").append(
                    $("<td>").text(user.id),
                    $("<td>").text(user.name),
                    $("<td>").text(user.lastName),
                    $("<td>").text(user.email),
                    $("<td>").text(new Date(user.birthDate).toLocaleDateString("en-US", { year: "numeric", month: "long", day: "2-digit" })),

                    $("<td>").text(user.phone || "N/A"),
                    $("<td>").text(user.country),
                    $("<td>").text(user.contactQuestion === true ? "Si" : "No"),
                    $("<td>").append(
                        $("<button>").addClass("btn btn-primary btn-sm me-2 btn-edit")
                            .data("model", user).text("Editar"),
                        $("<button>").addClass("btn btn-danger btn-sm btn-delete")
                            .data("txtId", user.id).text("Eliminar")
                    )
                ));
            });
        })
        .catch(error => console.error('Error fetching users:', error));
}
function populateCountries(countryString) {
    const cbxPais = $("#cbxPais");
    cbxPais.empty();

    let countryCode = null;
    if (countryString) {
        countryCode = countryString.split(" - ")[1];
    }

    fetch(uri + "getCountries")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(countries => {
            if (Array.isArray(countries) && countries.length > 0) {
                countries.forEach(country => {
                    const option = new Option(country.name, country.code);
                    cbxPais.append(option);

                    if (country.id === 1) { 
                        option.selected = true;
                    }
                });
            } else {
                cbxPais.append(new Option("No countries found", ""));
            }
        })
        .catch(error => {
            cbxPais.append(new Option("Error fetching countries", error));
        });
}
function ShowModal(model) {
    $("#txtId").val(model.id || 0);
    $("#txtNombre").val(model.name || "");
    $("#txtApellido").val(model.lastName || "");
    $("#txtCorreo").val(model.email || "");
    $("#txtFechaNacimiento").val(model.birthDate ? new Date(model.birthDate).toISOString().split("T")[0] : "");
    $("#txtTelefono").val(model.phone || "");

    $("#cbxPais").val(model.country || "Costa Rica");
    $('.modal').modal("show");
}
function clearFields() {
    $("#txtId, #txtNombre, #txtApellido, #txtCorreo, #txtFechaNacimiento, #txtTelefono").val('');
    $("#cbxPais").val("Costa Rica");
    $("#cbxRecibirInfo").val("false");
}
function createUser() {
    if (!validateFields()) {
        return;
    }

    let newUser = {
        Id: parseInt($("#txtId").val()) || 0,
        Name: $("#txtNombre").val().trim(),
        LastName: $("#txtApellido").val().trim(),
        Email: $("#txtCorreo").val().trim(),  
        BirthDate: new Date($("#txtFechaNacimiento").val()).toISOString(),
        Phone: $("#txtTelefono").val().trim(),
        Country: $("#cbxPais option:selected").text() + " - " + $("#cbxPais").val(), 
        ContactQuestion: $("#cbxRecibirInfo").val() === "true" ? true : false
    };

    console.log('Sending new user:', newUser);

    fetch("CreateUser", {  
        method: "POST",
        headers: {
            'Content-Type': 'application/json;charset=utf-8'
        },
        body: JSON.stringify(newUser)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Error: ${response.status} - ${response.statusText}`);
            }
            return response.text(); 
        })
        .then(() => {
            alert("Usuario agregado correctamente!");
            $('.modal').modal("hide");
            clearFields();
            getUsers();
        })
        .catch(error => {
            console.error("Error:", error);
            alert(`Error al crear usuario: ${error.message}`);
        });
}
function updateUser(user) {
    fetch(uri + 'UpdateUser', {
        method: "PUT",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    })
        .then(response => {
            if (response.ok) {
                return response.text();
            } else {
                return Promise.reject(response);
            }
        })
        .then(message => {
            alert(message || "Usuario actualizado correctamente!");
            $('.modal').modal("hide");
            clearFields();
            getUsers();
        })
        .catch(error => alert("Error al actualizar usuario."));
}
function validateFields() {
    if ($("#txtNombre").val().trim() === "") {
        alert("El nombre es obligatorio.");
        $("#txtNombre").trigger("focus");
        return false;
    }
    if ($("#txtApellido").val().trim() === "") {
        alert("El apellido es obligatorio.");
        $("#txtApellido").trigger("focus");
        return false;
    }
    if ($("#txtCorreo").val().trim() === "") {
        alert("El correo electrónico es obligatorio.");
        $("#txtCorreo").trigger("focus");
        return false;
    }
    return true;
}

$("#tbList tbody").on("click", ".btn-delete", function () {
    let id = $(this).data("txtId");
    if (confirm("¿Desea eliminar este usuario? ID: " + id)) {
        fetch(uri + 'DeleteUser/' + id, {  
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',  
            }
        })
            .then(response => {
                if (response.ok) {
                    alert("Usuario eliminado correctamente!");
                    getUsers();  
                } else {
                    alert("Error al eliminar usuario.");
                }
            })
            .catch(error => alert("Error al eliminar usuario."));  
    }
});

$("#btnNew").on("click", function () {
    isEditing = false;
    clearFields();
    ShowModal(userModel);
});

$("#tbList tbody").on("click", ".btn-edit", function () {
    isEditing = true;
    currentItem = $(this).data("model");
    ShowModal(currentItem);
});

$("#btnSave").on("click", function () {
    if (!validateFields()) return;

    let user = {
        id: $("#txtId").val(),
        name: $("#txtNombre").val(),
        lastName: $("#txtApellido").val(),
        email: $("#txtCorreo").val(),
        birthDate: $("#txtFechaNacimiento").val(),
        phone: $("#txtTelefono").val(),
        country: $("#cbxPais").val(),
        contactQuestion: $("#cbxRecibirInfo").val() === "true"
    };

    if (isEditing) {
        updateUser(user);
    } else {
        createUser(user);
    }
});

