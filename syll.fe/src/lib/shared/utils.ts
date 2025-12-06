import { CalendarDate } from "@internationalized/date";
import moment from "moment";

export class Utils{
     public static convertLowerCase(string: string = '') {
        if (string.length > 0) {
            return string.charAt(0).toLocaleLowerCase() + string.slice(1);
        }
        return '';
    }

    /**
     * đảo từ 1-12-2021 -> 2021-12-1
     * @param date
     * @returns
     */
    public static reverseDateString(date: string) {
        return date.split(/[-,/]/).reverse().join('-');
    }

    public static replaceAll(str: string, find: string, replace: string) {
        var escapedFind = find.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, '\\$1');
        return str.replace(new RegExp(escapedFind, 'g'), replace);
    }

    public static makeRandom(lengthOfCode: number = 100, possible?: string) {
        possible = 'AbBCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890-_';
        let text = '';
        for (let i = 0; i < lengthOfCode; i++) {
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        //
        return text;
    }

    static convertVietnameseToEng(str: string, isKeepCase = false) {
        // if(!isKeepCase) {
        //     str= str.toLowerCase();
        // }
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, 'a');
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, 'e');
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, 'i');
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, 'o');
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, 'u');
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, 'y');
        str = str.replace(/đ/g, 'd');

        str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, 'A');
        str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, 'E');
        str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, 'I');
        str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, 'O');
        str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, 'U');
        str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, 'Y');
        str = str.replace(/Đ/g, 'D');

        return str;
    }

    static formatDateCallApi(date: string | Date | CalendarDate | null | undefined, format = 'YYYY-MM-DDTHH:mm:ss') {
    if (!date) return '';
    
    // Xử lý CalendarDate
    if (date && typeof date === 'object' && 'year' in date && 'month' in date && 'day' in date) {
        const calendarDate = date as CalendarDate;
        const jsDate = new Date(calendarDate.year, calendarDate.month - 1, calendarDate.day);
        if (moment(jsDate).isValid()) {
            return moment(jsDate).format(format);
        }
    }
    
    // Xử lý string hoặc Date
    if (moment(date).isValid()) {
        return moment(date).format(format);
    }
    
    return '';
    }

    
    static formatDateView(date: string | Date | CalendarDate | null | undefined, format = 'DD/MM/YYYY') {
    if (!date) return '';
    
    // Xử lý CalendarDate
    if (date && typeof date === 'object' && 'year' in date && 'month' in date && 'day' in date) {
        const calendarDate = date as CalendarDate;
        const jsDate = new Date(calendarDate.year, calendarDate.month - 1, calendarDate.day);
        if (moment(jsDate).isValid()) {
            return moment(jsDate).format(format);
        }
    }
    
    // Xử lý string hoặc Date
    if (moment(date).isValid()) {
        return moment(date).format(format);
    }
    
    return '';
    }

    static  parseCalendarDate(dateStr: string | Date | null | undefined): CalendarDate | undefined {
		if (!dateStr) return undefined;
		const date = typeof dateStr === 'string' ? new Date(dateStr) : dateStr;
		return new CalendarDate(date.getFullYear(), date.getMonth() + 1, date.getDate());
	}

    static base64UrlEncode(arrayBuffer: ArrayBuffer) {
        let str = String.fromCharCode(...new Uint8Array(arrayBuffer));
        return btoa(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    }

}