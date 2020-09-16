var displayDate = function (date) {
    if (!date) return '';
    var time = moment.utc(date);
    var displayFormat = 'ddd - Do MMM YYYY hh:mm:ss A';
    return time.local().format(displayFormat);
}

var emailValidationRegex = /^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(db|gmail|google)\.com$/;