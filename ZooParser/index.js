var ContentManager = null;
var Condition = null;

$(function () {
    ContentManager = ContentManagerConstructor();
    Condition = ConditionConstructor();
});

$('#srcBtn').click(function () {
    Condition.Update();
    $.post('api/zoobase/database', { '': JSON.stringify(Condition) })
        .done(function (data) {
            ContentManager.Show(data);
        });
});

$('#updBtn').click(function () {
    $("#updBtn").attr('disabled', "disabled");
    $.getJSON('api/zoobase/updatedb')    
        .done(function (data) {
            $('#updBtn').removeAttr('disabled');
            if(data=="True")
                $('#notify').html("База данных обновлена!");
            else if (data=="False")
                $('#notify').html("Ошибка в обновлении базы данных!");
    });
});

function ConditionConstructor() {
    var o = new Object();

    o.CostRange = "0-0";
    o.DateRange = "01.01.2000-01.01.2000";
    o.Animal = "All";
    o.Deal = "All";
    o.Update = function () {
        this.CostRange = $('#costFrom').val() + "-" + $('#costTo').val();
        this.DateRange = $('#dateFrom').val() + "-" + $('#dateTo').val();
        this.Animal = getAnimal();
        this.Deal = getDeal();
    }
    return o;
};

function getAnimal() {
    switch($('#animalType').val())
    {    
        case "Собаки": return "dogs";
        case "Кошки": return "cats";
        case "Птицы": return "birds";
        case "Грызуны": return "rodents";
        case "Рептилии": return "reptile";
        case "Рыбки": return "fish";
        case "Насекомые": return "insects";
        case "Лошади": return "horses";
        case "Домашний скот": return "Home";
        case "Экзотические": return "exotic";
    }
}

function getDeal() {
    switch ($('#dealType').val()) {
        case "Продать": return "sale";
        case "Купить": return "buy";
        case "Вязка": return "love";
        case "Отдать": return "free";
    }
}

function ContentManagerConstructor() {
    var o = new Object();    
    o.Show = function (data) {
        var html = "";
        for (var i = 0; i < data.length; i++) {
            html+="<div style='border:solid black 1px;'>"
            html += "<div><h2>" + data[i].Title + "</h2></div><br/>";
            html += "<div><strong>Дата объявления:</strong>" + data[i].Date + "</div><br/>";
            html += "<div><img src='" + data[i].ImageUrl + "'/></div><br/>";
            html += "<div><strong>Подробности:</strong>" + data[i].Information + "</div><br/>";
            html += "<div><strong>Цена:</strong>" + data[i].Cost + "</div><br/>";
            html += "<div><strong>Автор:</strong>" + data[i].Autor + "</div><br/>";
            html += "<div><strong>Телефон:</strong>" + data[i].Number + "</div><br/>";
            html += "</div>"
        }
        if (html == "")
            $('#notify').html("Поиск не дал результатов, попробуйте обновить базу данных!");
        $('#content').html(html);
    };

    return o;
}