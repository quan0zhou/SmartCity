
  // 返回res.data的interface
  export interface IResponse<T = any> {
    data?: T;
    msg?: string;
    status: boolean;
  }
  
