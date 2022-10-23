import { get,post } from '@/utils/http/axios';
enum URL {
  list = '/order/list',
  info = '/order/info',
  create='/order/create',
  remove='/order/remove',
  refund='/order/refund',
  pay='/order/pay'
}


const getOrderList = async (id:string,openid:string,status:string) => get<any>({ url: URL.list+'/'+id+'/'+openid+'/'+status });
const getOrderInfo = async (id:string) => get<any>({ url: URL.info+'/'+id });
const createOrder = async (data:any) => post<any>({ url: URL.create,data:data });
const cancelOrder = async (id:string) => get<any>({ url: URL.remove+'/'+id});
const refundOrder = async (id:string) => post<any>({ url: URL.refund+'/'+id});
const payOrder = async (id:string) => post<any>({ url: URL.pay+'/'+id});
export { getOrderList,getOrderInfo,createOrder,cancelOrder,refundOrder,payOrder };