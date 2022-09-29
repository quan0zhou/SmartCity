import { get } from '@/utils/http/axios';
enum URL {
  list = '/order/list'
}


const getOrderList = async (id:string,status:string) => get<any>({ url: URL.list+'/'+id+'/'+status });
export { getOrderList };