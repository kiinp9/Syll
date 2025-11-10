export interface IViewFormLayout {
	id?: number;
	idFormLoai?: number;
	ten?: string;
	order?: number;
	class?: string;
	style?: string;
	items?: IViewFormBlock[];
}

export interface IViewFormBlock {
	id?: number;
	order?: number;
	class?: string;
	style?: string;
	items?: IViewFormRow[];
}

export interface IViewFormRow {
	id?: number;
	order?: number;
	class?: string;
	style?: string;
	items: IViewFormItem[];
}

export interface IViewFormItem {
	id?: number;
	order?: number;
	type?: number;
	ratio?: number;
	class?: string;
	style?: string;
	inputName?: string;
	items?: IViewItemTruongData[];
	headers?: IViewTblHeader[];
}

export interface IViewItemTruongData {
	id?: number;
	tenTruong?: string;
	type?: string;
	class?: string;
	style?: string;
	item: IViewItemData;
	items?: IViewFormSelectOption[];
}

export interface IViewItemData {
	id?: number;
	data?: string;
	indexRowTable?: null;
}

export interface IViewFormSelectOption {
	id?: number;
	data: string;
	order?: number;
	class?: string;
	style?: string;
}

export interface IViewTblHeader {
	id?: number;
	data: string;
	idTruongData ?: number;
	order?: number;
	ratio?: number
	type?: string;
	class?: string;
	style?: string;
}