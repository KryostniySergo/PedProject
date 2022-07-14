import {$authHost, $host} from "./index";

export const createType = async (type) =>{
    const {data} = await $authHost.post('Type/Create', type)
    return data
}

export const fetchTypes = async () =>{
    const {data} = await $host.get('Type/GetAll')
    return data
}

export const createBrand = async (brand) =>{
    const {data} = await $authHost.post('Brand/Create', brand)
    return data
}

export const fetchBrand = async () =>{
    const {data} = await $host.get('Brand/GetAll')
    return data
}

export const createDevice = async (device) =>{
    const {data} = await $authHost.post('Device/Create', device)
    return data
}

export const fetchDevice = async (typeId, brandId, page, limit = 5) =>{
    const {data} = await $host.get('Device/GetAll', {params: {
            typeId, brandId, page, limit
        }})
    return data
}

export const fetchOneDevice = async (id) =>{
    const {data} = await $host.get('Device/GetOne/' + id)
    return data
}