function addNewAnswerForAdd() {
    var container = document.querySelectorAll('#add-answers .form-check')[0].cloneNode();
    var input = document.querySelector('#add-answers .form-check input[type="text"]').cloneNode();
    var check = document.querySelector('#add-answers .form-check input[type="checkbox"]').cloneNode();
    input.value = '';
    var btn = document.querySelector('#add-answers .form-check i').cloneNode();
    var div = document.querySelector('#add-answers .form-check div').cloneNode();
    div.appendChild(check);
    div.appendChild(input);
    div.appendChild(btn);
    container.appendChild(div);
    document.querySelector('#add-answers').appendChild(container);
}
function deleteAnswer(element) {
    if (element.parentElement.parentElement.parentElement.children.length > 1) {
        element.parentElement.parentElement.outerHTML = "";
    } else {
        alert("Must exist at least 1 answer!");
    }
}

function addNewAnswerForEdit() {
    var container = document.querySelectorAll('#edit-answers .form-check')[0].cloneNode();
    var input = document.querySelector('#edit-answers .form-check input[type="text"]').cloneNode();
    var check = document.querySelector('#edit-answers .form-check input[type="checkbox"]').cloneNode();
    input.value = '';
    var btn = document.querySelector('#edit-answers .form-check i').cloneNode();
    var inputId = document.querySelector('#edit-answers .form-check input[type="hidden"]').cloneNode();
    inputId.value = 0;
    var div = document.querySelector('#edit-answers .form-check div').cloneNode();
    div.appendChild(check);
    div.appendChild(input);
    div.appendChild(btn);
    div.appendChild(inputId);
    container.appendChild(div);
    document.querySelector('#edit-answers').appendChild(container);
}

$(".icon-edit").on("click", function () {
    let questionId = $(this).attr("data-question-id");
    let questionContent = $(this).attr("data-question-content");
    let questionLevel = $(this).attr("data-question-level");
    let questionScore = $(this).attr("data-question-score");
    let questionAnswersId = $(this).attr("data-question-answersId");
    let questionMarks = $(this).attr("data-question-marks");
    let questionAnswers = $(this).attr("data-question-answers");

    $("#editQuestionId").val(questionId);
    $("#editQuestionContent").val(questionContent);
    $("#editQuestionLevel").val(questionLevel);
    $("#editQuestionScore").val(questionScore);

    var listMarks = questionMarks.split("|||");
    var listAnswers = questionAnswers.split("|||");
    var listAnswersId = questionAnswersId.split("|||");
    var container = document.querySelector("#edit-answers");
    container.innerHTML = "";
    for (var i = 0; i < listMarks.length; i++) {
        var isCheck = listMarks[i] == "true" ? "checked" : "";
        container.innerHTML += `<div class="form-check">
                                        <div style="display: flex; justify-content: space-between" class="mb-2">
                                            <input type="hidden" class="edit-answer-answerid" value="${listAnswersId[i]}">
                                            <input class="form-check-input edit-check-mark" type="checkbox" ${isCheck}>
                                            <input class="form-control form-control-sm edit-answer-text" type="text" name="objectives" required value="${listAnswers[i]}"
                                                   style="width: 90%">
                                            <i style="margin: auto 0;" onclick="deleteAnswer(this)"
                                               class="text-danger fa-solid fa-circle-minus"></i>
                                        </div>
                                    </div> `
    }
    $("#editQuestion").modal("show");
});

function deleteQuestionById(id, btn) {
    $.confirm({
        title: 'Confirm!',
        content: 'Delete this question!',
        type: 'red',
        buttons: {
            delete: {
                btnClass: 'btn-red',
                action: function () {
                    $.ajax({
                        url: `../../Question/DeleteQuestion?id=${id}`,
                        type: 'DELETE',
                        success: function (data, textStatus, jqXHR) {
                            $(btn).closest('div').fadeOut("slow");
                        },
                    });
                }
            },
            cancel: function () {
            }
        }
    });
}
function submitFormAddQuestion() {


    var listCheckMarks = document.querySelectorAll(".add-check-mark");
    var listMarks = new Array();
    for (var i = 0; i < document.getElementById("add-answers").children.length; i++) {
        if (listCheckMarks[i].checked == true) {
            listMarks[i] = "true";
        } else {
            listMarks[i] = "false";
        }
    }
    var listAnswers = document.querySelectorAll(".add-answer-text");
    var txtAnswers = new Array();;
    for (var i = 0; i < listAnswers.length; i++) {
        txtAnswers[i] = listAnswers[i].value
    }

    $(function () {
        $.ajax({
            type: "POST",
            url: "../../Question/CreateQuestion",
            cache: false,
            data: {
                quizId: document.getElementById("quizId").value,
                questionContent: document.getElementById("questionContent").value,
                questionLevel: document.getElementById("questionLevel").value,
                questionScore: document.getElementById("questionScore").value,
                answersMark: listMarks,
                answers: txtAnswers
            },
            success: function (result) {
                location.reload();
            }
        });
    });

}

function submitFormEditQuestion() {


    var listCheckMarks = document.querySelectorAll(".edit-check-mark");
    var listMarks = new Array();
    for (var i = 0; i < document.getElementById("edit-answers").children.length; i++) {
        if (listCheckMarks[i].checked == true) {
            listMarks[i] = "true";
        } else {
            listMarks[i] = "false";
        }
    }
    var listAnswers = document.querySelectorAll(".edit-answer-text");
    var txtAnswers = new Array();;
    for (var i = 0; i < listAnswers.length; i++) {
        txtAnswers[i] = listAnswers[i].value
    }
    var listAnswersId = document.querySelectorAll(".edit-answer-answerid");
    var answersId = new Array();;
    for (var i = 0; i < listAnswersId.length; i++) {
        answersId[i] = listAnswersId[i].value
    }
    $(function () {
        $.ajax({
            type: "POST",
            url: "../../Question/UpdateQuestion",
            cache: false,
            data: {
                quizId: document.getElementById("quizId").value,
                questionId: document.getElementById("editQuestionId").value,
                questionContent: document.getElementById("editQuestionContent").value,
                questionLevel: document.getElementById("editQuestionLevel").value,
                questionScore: document.getElementById("editQuestionScore").value,
                answersMark: listMarks,
                answers: txtAnswers,
                answersId: answersId
            },
            success: function (result) {
                location.reload();
            }
        });
    });
}

