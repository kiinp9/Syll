export interface IFormConfig {
    type?: string
    id?: string
    children?: IFormBlockConfig[]
}

export interface IFormBlockConfig {
    type: string
    id: string
    children: IFormRowConfig[]
}

export interface IFormRowConfig {
    type: string
    id: string
    children: IFormItemConfig[]
}

export interface IFormItemConfig {
    type: string
    id: string
}