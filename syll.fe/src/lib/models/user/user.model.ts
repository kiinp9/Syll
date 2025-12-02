export interface IViewUserMe {
	id?: string;
	userName?: string;
	email?: string;
	msAccount?: string;
	phoneNumber?: string | null;
	fullName?: string;
	emailConfirmed?: boolean;
	phoneNumberConfirmed?: boolean;
	createdAt?: string; // or Date if you plan to parse it
	roles?: string[];
}
