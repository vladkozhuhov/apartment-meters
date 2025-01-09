using System.Net;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace API.ODataControllers;

/// <summary>
/// Контролер для работы с сущностями
/// </summary>
public class EntityController : ODataController
{
    // /// <summary>
    // /// Пользователи
    // /// </summary>
    // /// <response code="200">Успешное выполнение.</response>
    // /// <response code="500">Ошибка сервера.</response>
    // [HttpPost("entity/user")]
    // [ProducesResponseType((int)HttpStatusCode.OK)]
    // [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    // [EnableQuery(
    //     HandleNullPropagation = HandleNullPropagationOption.False,
    //     MaxAnyAllExpressionDepth = 5,
    //     EnableConstantParameterization = false
    // )]
    // public async Task<IActionResult> GetWagon(ODataQueryOptions<UserEntity> query)
    //     => Ok(await Mediator.Send(new GetUserQuery(query)));
}