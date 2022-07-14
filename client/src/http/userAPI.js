import {$authHost, $host} from "./index";
import jwt_decode from 'jwt-decode'

export const registration = async (email, password) =>{
    const {data} = await $host.post('User/Registration', {email, password, role: 'ADMIN'})
    localStorage.setItem('token', data.acces_token)
    return jwt_decode(data.acces_token)
}

export const login = async (email, password) =>{
    const {data} = await $host.post('User/Login', {email, password})
    localStorage.setItem('token', data.acces_token)
    return jwt_decode(data.acces_token)
}
export const check = async () =>{
    const {data} = await $authHost.post('User/Auth')
    localStorage.setItem('token', data.acces_token)
    return jwt_decode(data.acces_token)
}