import { get } from '@/utils/http/axios';
enum URL {
  list = '/order/list'
}


const getOrderList = async (id:string) => get<any>({ url: URL.list+'/'+id });
export { getOrderList };