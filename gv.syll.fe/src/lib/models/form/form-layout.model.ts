export interface IViewFormLayout {
	id?: number;
	idFormLoai?: number;
	ten?: string;
	order?: number;
	items?: IViewFormBlock[];
}

export interface IViewFormBlock {
	id?: number;
	order?: number;
	items?: IViewFormRow[];
}

export interface IViewFormRow {
	id?: number;
	order?: number;
	items: IViewFormItem[];
}

export interface IViewFormItem {
	id?: number;
	order?: number;
	type?: number;
	ratio?: number;
	items?: IViewItemTruongData[];
}

export interface IViewItemTruongData {
	id?: number;
	tenTruong?: string;
	type?: string;
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
